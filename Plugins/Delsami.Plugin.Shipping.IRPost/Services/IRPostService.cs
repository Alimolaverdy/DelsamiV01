using Newtonsoft.Json;
using Delsami.Plugin.Misc.Core.Models.Resilience;
using Delsami.Plugin.Misc.Core.Services.ApiCaller;
using Delsami.Plugin.Shipping.Core.Helpers;
using Delsami.Plugin.Shipping.Core.Services;
using Delsami.Plugin.Shipping.IRPost.Models;
using Delsami.Plugin.Widgets.AddressLocation.Services;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Services;

public class IRPostService : IIRPostService
{
    #region Fields

    private readonly IShippingService _shippingService;
    private readonly ILogger _logger;
    private readonly IRPostSettings _settings;
    private readonly IProductService _productService;
    private readonly IPriceRangeService _priceRangeService;
    private readonly IApiCaller _apiCaller;
    private readonly IAddressCityService _addressCityService;
    private readonly IDelsamiPostCityMappingService _DelsamiPostCityMappingService;
    private readonly ICartonService _cartonService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    ResilienceOptions resilienceOptions = new()
    {
        EnableCircuitBreaker = false,
        EnableRetry = true,
        RetryCount = 3,
        RetryBaseDelaySeconds = 2,
        EnableTimeout = true,
        TimeoutSeconds = 10
    };

    #endregion

    #region Ctor
    public IRPostService(IRPostSettings iRPostSettings,
                         IApiCaller apiCaller,
                         ILogger logger,
                         IAddressCityService addressCityService,
                         IDelsamiPostCityMappingService DelsamiPostCityMappingService,
                         IShippingService shippingService,
                         ICartonService cartonService,
                         IProductService productService,
                         IPriceRangeService priceRangeService,
                         IOrderTotalCalculationService orderTotalCalculationService)
    {

        _settings = iRPostSettings;
        _addressCityService = addressCityService;
        _DelsamiPostCityMappingService = DelsamiPostCityMappingService;
        _apiCaller = apiCaller;
        _shippingService = shippingService;
        _productService = productService;
        _priceRangeService = priceRangeService;
        _logger = logger;
        _cartonService = cartonService;
        _orderTotalCalculationService = orderTotalCalculationService;
    }

    #endregion

    #region Methods

    public async Task<IList<ShippingOption>> PrepareShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        var options = new List<ShippingOption>();
        decimal totalWeight = await CalculateTotalWeightAsync(getShippingOptionRequest);
        var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(_settings.PostShippingMethodId);

        decimal shippingCost = 0;
        var requests = await prepareInquiryRequests(totalWeight, getShippingOptionRequest);

        if (requests.Any())
        {
            var apiIsSuccess = true;

            foreach (var request in requests)
            {
                var iRPostCostResult = await GetBarCodeAsync(request);
                if (iRPostCostResult.IsCallSuccess && iRPostCostResult.Response != null && iRPostCostResult.Response.Status)
                {
                    shippingCost += iRPostCostResult.Response.data.Totalprice;
                }
                else
                {
                    if (!_settings.EnableManualCalc)
                        return options;

                    apiIsSuccess = false;
                }
            }


            if (!apiIsSuccess)
                shippingCost = await CalculateShippingCostAsync(totalWeight, getShippingOptionRequest.ShippingAddress.StateProvinceId.Value);


            options.Add(new ShippingOption
            {

                Name = shippingMethod.Name,
                Description = shippingMethod.Description,
                Rate = Math.Ceiling(shippingCost / 1000m) * 1000m,
                DisplayOrder = shippingMethod.DisplayOrder,

            });
        }
        return options;
    }

    public async Task<(bool IsCallSuccess, GetBarCodeResponse Response)> GetBarCodeAsync(GetBarCodeRequest getPriceRequest)
    {
        var url = _settings.BaseUrl + IRPostEndpoints.GetBarCode;
        try
        {
            var response = await _apiCaller.PostAsync<GetBarCodeRequest, GetBarCodeResponse>(url, getPriceRequest, resilienceOptions: resilienceOptions);

            if (!response.IsSuccess || !response.Data.Status)
            {
                await _logger.ErrorAsync($"Post Result was unsuccessfuly due to: {JsonConvert.SerializeObject(response)}");
                return (false, null);
            }

            return (true, response.Data);
        }

        catch (Exception ex)
        {
            await _logger.ErrorAsync(ex.Message, ex);
            return (false, null);
        }
    }

    #endregion

    #region Internal Methods

    private async Task<decimal> CalculateTotalWeightAsync(GetShippingOptionRequest request)
    {
        decimal totalWeight = 0;

        foreach (var packageItem in request.Items)
        {
            var shoppingCartItem = packageItem.ShoppingCartItem;

            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
            if (product == null)
                continue;

            totalWeight += (product.Weight) * packageItem.GetQuantity();
        }

        return totalWeight;
    }


    private async Task<decimal> CalculateShippingCostAsync(decimal weight, int stateProvinceId)
    {
        var isNeighbour = _settings.NeighbouringProvinces?
                        .Split(',')
                        .Select(p => int.TryParse(p.Trim(), out var id) ? id : (int?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .Contains(stateProvinceId) ?? false;

        var price = await GetShippingPriceByWeightAsync(weight, isNeighbour);

        return price;
    }

    private async Task<decimal> GetShippingPriceByWeightAsync(decimal weight, bool isNeighbour)
    {
        weight = weight / 1000m;
        var price = await _priceRangeService.GetPriceRangeAsync(weight, isNeighbour);

        return price ?? 0;

    }


    private async Task<List<GetBarCodeRequest>> prepareInquiryRequests(decimal totalWeight, GetShippingOptionRequest getShippingOption)
    {
        var OrderTotal = await _orderTotalCalculationService.GetShoppingCartSubTotalsAsync(getShippingOption.Items.Select(a => a.ShoppingCartItem).ToList());

        var cartons = await _cartonService.GetCartonsAsync();

        if (cartons is null && !cartons.Any())
            return null;

        var products = getShippingOption.Items
                      .SelectMany(item => Enumerable.Repeat(item.Product, item.GetQuantity()))
                      .ToList();

        var packingResult = PackagingHelper.PackProducts(cartons, products);

        int properBoxNum;
        if (packingResult.Any(a => a.Carton.PostBoxNumber == 0))
        {
            properBoxNum = 10; //10 is the 'other' box in Post.
        }
        else
        {

            var length = packingResult.Select(s => s.Carton).Sum(s => s.Length);
            var width = packingResult.Select(s => s.Carton).Sum(s => s.Width);
            var height = packingResult.Select(s => s.Carton).Sum(s => s.Height);

            properBoxNum = cartons.FirstOrDefault(s => s.Length >= length && s.Width >= width && s.Height >= height)?.PostBoxNumber ?? 10;

        }

        var addressCity = await _addressCityService.GetAddressCityByAddressIdAsync(getShippingOption.ShippingAddress.Id);

        if (addressCity is null)
            return new List<GetBarCodeRequest>();

        var originDelsamiPostCityMapping = await _DelsamiPostCityMappingService.GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(addressCity.CityId);

        if (originDelsamiPostCityMapping is null)
            return new List<GetBarCodeRequest>();

        var total = (int)Math.Ceiling(totalWeight);

        var packageWeights = Enumerable
            .Repeat(40000, total / 40000)
            .Append(total % 40000)
            .Where(x => x > 0)
            .ToList();

        var requests = packageWeights.Select(weight => new GetBarCodeRequest
        {
            username = _settings.User,
            password = _settings.Password,
            contractcode = _settings.ContractCode,
            postnodecode = _settings.PostNodeCode,
            typecode = 7,
            servicetype = 5,
            parceltype = 18,
            sourcecode = 1,
            destcode = originDelsamiPostCityMapping.PostCityId,
            receivername = "استعلام",
            receiverpostalcode = "",
            weight = weight,
            receiverid = "",
            receivermobile = "",
            receiveraddress = "استعلام",
            insurancetype = 5,
            insuranceamount = (long)OrderTotal.subTotalWithDiscountInclTax,
            spsreceivertimetype = 0,
            tlsservicetype = 0,
            tworeceiptant = false,
            electroreceiptant = true,
            iscot = false,
            smsservice = true,
            isnonstandard = properBoxNum == 10,
            sendplacetype = 0,
            Boxsize = properBoxNum,
            PriceInquiry = 1,
            Podcode = ""
        }).ToList();

        return requests;
    }
    #endregion

}

