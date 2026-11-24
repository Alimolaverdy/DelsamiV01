using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Services
{
    public class ZarIranLogisticService : IZarIranLogisticService
    {

        #region Fields
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor
        public ZarIranLogisticService(ISettingService settingService, ILocalizationService localizationService, IShippingService shippingService, IProductService productService, ILogger logger)
        {
            _settingService = settingService;
            _shippingService = shippingService;
            _localizationService = localizationService;
            _productService = productService;
            _logger = logger;
        }
        #endregion

        #region Methods

        public async Task<IList<ShippingOption>> PrepareShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            var shippingOptions = new List<ShippingOption>();

            var settings = await _settingService.LoadSettingAsync<ZarIranLogisticSettings>();

            var (totalWeight, totalVolume, hasOversizedProduct) = await CalculatePackageMetricsAsync(getShippingOptionRequest, settings);
            bool mustUsePickupTruck = ShouldUsePickupTruck(totalWeight, totalVolume, hasOversizedProduct, settings);

            var rate = mustUsePickupTruck
                ? settings.PickupTruckPrice
                : settings.Price;

            var schaduleRate = mustUsePickupTruck
                ? settings.ScheduledPriceForHeavyOrBigBasket
                : settings.ScheduledPrice;

            var methodId = mustUsePickupTruck
                ? settings.PickupTruckShippingMethodId
                : settings.ShippingMethodId;

            var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(methodId);
            if (shippingMethod == null)
            {
                _logger.Information($"shippingMethod : {methodId} Is Null ");
                return shippingOptions;
            }


            shippingOptions.Add(new ShippingOption
            {
                Name = shippingMethod.Name,
                Description = shippingMethod.Description,
                Rate = rate,
				DisplayOrder = shippingMethod.DisplayOrder,

			});

            var scheduledShippingMethod = mustUsePickupTruck? await _shippingService.GetShippingMethodByIdAsync(settings.ScheduledShippingMethodWithTruckId):
                                                              await _shippingService.GetShippingMethodByIdAsync(settings.ScheduledShippingMethodId);

            if (scheduledShippingMethod is not null)
            {
                shippingOptions.Add(new ShippingOption
                {
                    Name = scheduledShippingMethod.Name,
                    Description = scheduledShippingMethod.Description,
                    Rate = schaduleRate,
					DisplayOrder = scheduledShippingMethod.DisplayOrder,

				});
            }

            return shippingOptions;
        }

        #endregion

        #region Internal Methods

        private async Task<(decimal totalWeight, decimal totalVolume, bool hasOversizedProduct)> CalculatePackageMetricsAsync(GetShippingOptionRequest request, ZarIranLogisticSettings settings)
        {
            decimal totalWeight = 0;
            decimal totalVolume = 0;
            bool hasOversizedProduct = false;
            var overSize = settings.OverSizeProductVolumeMustCarryByPickupTruckInCM3;

            foreach (var packageItem in request.Items)
            {
                var shoppingCartItem = packageItem.ShoppingCartItem;
                var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
                if (product == null)
                    continue;

                var sourceProduct = product;
                var quantity = packageItem.GetQuantity();

                totalWeight += sourceProduct.Weight * quantity;

                var productVolume = sourceProduct.Length * sourceProduct.Width * sourceProduct.Height;
                totalVolume += productVolume * quantity;

                if (sourceProduct.Length > overSize || sourceProduct.Width > overSize || sourceProduct.Height > overSize)
                    hasOversizedProduct = true;
            }
            return (totalWeight, totalVolume, hasOversizedProduct);
        }

        private bool ShouldUsePickupTruck(decimal totalWeight, decimal totalVolume, bool hasOversizedProduct, ZarIranLogisticSettings settings)
        {
            return hasOversizedProduct
                   || totalVolume > settings.VolumeMustCarryByPickupTruckInCM3
                   || totalWeight > settings.WeightMustCarryByPickupTruckInGrams;
        }

        #endregion

    }
}