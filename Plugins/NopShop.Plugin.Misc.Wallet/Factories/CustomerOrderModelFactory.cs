using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NopShop.Plugin.Misc.Wallet.Factories;

public class CustomOrderModelFactory : OrderModelFactory, IOrderModelFactory
{
	#region Fields

	private readonly IDateTimeHelper _dateTimeHelper;
	private readonly ILocalizationService _localizationService;
	private readonly IOrderTotalCalculationService _orderTotalCalculationService;
	private readonly IPriceFormatter _priceFormatter;
	private readonly IRewardPointService _rewardPointService;
	private readonly IStoreContext _storeContext;
	private readonly ICurrencyService _currencyService;
	private readonly IWorkContext _workContext;
	private readonly RewardPointsSettings _rewardPointsSettings;

	#endregion

	#region Ctor

	public CustomOrderModelFactory(AddressSettings addressSettings,
			CatalogSettings catalogSettings,
			IAddressModelFactory addressModelFactory,
			IAddressService addressService,
			ICountryService countryService,
			ICurrencyService currencyService,
			ICustomerService customerService,
			IDateTimeHelper dateTimeHelper,
			IGiftCardService giftCardService,
			ILocalizationService localizationService,
			IOrderProcessingService orderProcessingService,
			IOrderService orderService,
			IOrderTotalCalculationService orderTotalCalculationService,
			IPaymentPluginManager paymentPluginManager,
			IPaymentService paymentService,
			IPictureService pictureService,
			IPriceFormatter priceFormatter,
			IProductService productService,
			IRewardPointService rewardPointService,
			IShipmentService shipmentService,
			IStateProvinceService stateProvinceService,
			IStaticCacheManager staticCacheManager,
			IStoreContext storeContext,
			IUrlRecordService urlRecordService,
			IVendorService vendorService,
			IWebHelper webHelper,
			IWorkContext workContext,
			MediaSettings mediaSettings,
			OrderSettings orderSettings,
			PdfSettings pdfSettings,
			RewardPointsSettings rewardPointsSettings,
			ShippingSettings shippingSettings,
			TaxSettings taxSettings,
			VendorSettings vendorSettings) : base(addressSettings,
			 catalogSettings,
			 addressModelFactory,
			 addressService,
			 countryService,
			 currencyService,
			 customerService,
			 dateTimeHelper,
			 giftCardService,
			 localizationService,
			 orderProcessingService,
			 orderService,
			 orderTotalCalculationService,
			 paymentPluginManager,
			 paymentService,
			 pictureService,
			 priceFormatter,
			 productService,
			 rewardPointService,
			 shipmentService,
			 stateProvinceService,
			 staticCacheManager,
			 storeContext,
			 urlRecordService,
			 vendorService,
			 webHelper,
			 workContext,
			 mediaSettings,
			 orderSettings,
			 pdfSettings,
			 rewardPointsSettings,
			 shippingSettings,
			 taxSettings,
			 vendorSettings)
	{

		_dateTimeHelper = dateTimeHelper;
		_priceFormatter = priceFormatter;
		_currencyService = currencyService;
		_localizationService = localizationService;
		_orderTotalCalculationService = orderTotalCalculationService;
		_rewardPointService = rewardPointService;
		_workContext = workContext;
		_storeContext = storeContext;
		_rewardPointsSettings = rewardPointsSettings;
	}

	#endregion


	public async override Task<CustomerRewardPointsModel> PrepareCustomerRewardPointsAsync(int? page)
	{
		//get reward points history
		var customer = await _workContext.GetCurrentCustomerAsync();
		var store = await _storeContext.GetCurrentStoreAsync();
		var pageSize = _rewardPointsSettings.PageSize;
		var rewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(customer.Id, store.Id, true, pageIndex: --page ?? 0, pageSize: pageSize);

		var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

		//prepare model
		var model = new CustomerRewardPointsModel
		{
			RewardPoints = await rewardPoints.SelectAwait(async historyEntry =>
			{
				var activatingDate = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
				return new CustomerRewardPointsModel.RewardPointsHistoryModel
				{
					Points =(int) await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(historyEntry.Points, currentCurrency),
					PointsBalance = historyEntry.PointsBalance.HasValue ? historyEntry.PointsBalance.ToString()
						: string.Format(await _localizationService.GetResourceAsync("RewardPoints.ActivatedLater"), activatingDate),
					Message = historyEntry.Message,
					CreatedOn = activatingDate,
					EndDate = !historyEntry.EndDateUtc.HasValue ? null :
						(DateTime?)(await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.EndDateUtc.Value, DateTimeKind.Utc))
				};
			}).ToListAsync(),

		
			PagerModel = new PagerModel(_localizationService)
			{
				PageSize = rewardPoints.PageSize,
				TotalRecords = rewardPoints.TotalCount,
				PageIndex = rewardPoints.PageIndex,
				ShowTotalSummary = true,
				RouteActionName = "CustomerRewardPointsPaged",
				UseRouteLinks = true,
				RouteValues = new RewardPointsRouteValues { PageNumber = page ?? 0 }
			}
		};

		//current amount/balance
		var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);
		var rewardPointsAmountBase = await _orderTotalCalculationService.ConvertRewardPointsToAmountAsync(rewardPointsBalance);
		var rewardPointsAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rewardPointsAmountBase, currentCurrency);
		model.RewardPointsBalance = rewardPointsBalance;
		//model.RewardPointsAmount = await _priceFormatter.FormatPriceAsync(Math.Round(rewardPointsAmount, 0, MidpointRounding.AwayFromZero), true, false);
		model.RewardPointsAmount = await _priceFormatter.FormatPriceAsync(rewardPointsAmount, true, false);

		//minimum amount/balance
		var minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
		var minimumRewardPointsAmountBase = await _orderTotalCalculationService.ConvertRewardPointsToAmountAsync(minimumRewardPointsBalance);
		var minimumRewardPointsAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(minimumRewardPointsAmountBase, currentCurrency);
		model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
		model.MinimumRewardPointsAmount = await _priceFormatter.FormatPriceAsync(minimumRewardPointsAmount, true, false);

		return model;
	}

}
