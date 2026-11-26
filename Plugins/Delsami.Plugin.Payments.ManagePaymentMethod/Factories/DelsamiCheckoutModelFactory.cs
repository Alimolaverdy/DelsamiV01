using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Payments.ManagePaymentMethod.Services;
using Delsami.Plugin.Widgets.AddressLocation.Services;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Delsami.Plugin.Misc.Core.Services.ShippingMethodService;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Factories
{
    public class DelsamiCheckoutModelFactory : CheckoutModelFactory, ICheckoutModelFactory
	{
		#region Fields

		private readonly IWorkContext _workContext;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly RewardPointsSettings _rewardPointsSettings;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IOrderTotalCalculationService _orderTotalCalculationService;
		private readonly IPriceFormatter _priceFormatter;
		private readonly IRewardPointService _rewardPointService;
		private readonly IOrderProcessingService _orderProcessingService;
		private readonly IPaymentPluginManager _paymentPluginManager;
		private readonly ILocalizationService _localizationService;
		private readonly IPaymentMethodCustomerRoleRestrictionService _roleRestrictionService;
		private readonly PaymentSettings _paymentSettings;
		private readonly IPaymentService _paymentService;
		private readonly ITaxService _taxService;
		private readonly ICurrencyService _currencyService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly ICustomerService _customerService;
		private readonly IPaymentMethodRestrictionService _paymentMethodRestrictionService;
		private readonly IPaymentMethodAmountRestrictionService _paymentMethodAmountRestrictionService;
		private readonly CoreSettings _coreSettings;
		private readonly IAddressCityService _addressCityService;
		private readonly IPaymentMethodShippingRestrictionService _paymentMethodShippingRestrictionService;
		private readonly IShippingMethodService _shippingMethodService;


		#endregion

		#region Ctor

		public DelsamiCheckoutModelFactory(AddressSettings addressSettings,
											CaptchaSettings captchaSettings,
											CommonSettings commonSettings,
											IAddressCityService addressCityService,
											IAddressModelFactory addressModelFactory,
											IAddressService addressService,
											IProductService productService,
                                            IShippingMethodService shippingMethodService,
											ICountryService countryService,
											IPaymentMethodShippingRestrictionService paymentMethodShippingRestrictionService,
											ICurrencyService currencyService,
											IPaymentMethodCustomerRoleRestrictionService roleRestrictionService,
											ICustomerService customerService,
											IGenericAttributeService genericAttributeService,
											ILocalizationService localizationService,
											IOrderProcessingService orderProcessingService,
											IOrderTotalCalculationService orderTotalCalculationService,
											IPaymentPluginManager paymentPluginManager,
											IPaymentService paymentService,
											IPickupPluginManager pickupPluginManager,
											IPriceFormatter priceFormatter,
											IPaymentMethodAmountRestrictionService paymentMethodAmountRestrictionService,
											IRewardPointService rewardPointService,
											IShippingPluginManager shippingPluginManager,
											IShippingService shippingService,
											IShoppingCartService shoppingCartService,
											IStateProvinceService stateProvinceService,
											IStoreContext storeContext,
											IStoreMappingService storeMappingService,
											ITaxService taxService,
											IWorkContext workContext,
											OrderSettings orderSettings,
											PaymentSettings paymentSettings,
											RewardPointsSettings rewardPointsSettings,
											ShippingSettings shippingSettings,
											TaxSettings taxSettings,
											IPaymentMethodRestrictionService paymentMethodRestrictionService,
											CoreSettings coreSettings) : base(addressSettings,
																			captchaSettings,
																			commonSettings,
																			addressModelFactory,
																			addressService,
																			countryService,
																			currencyService,
																			customerService,
																			genericAttributeService,
																			localizationService,
																			orderProcessingService,
																			orderTotalCalculationService,
																			paymentPluginManager,
																			paymentService,
																			pickupPluginManager,
																			priceFormatter,
																			rewardPointService,
																			shippingPluginManager,
																			shippingService,
																			shoppingCartService,
																			stateProvinceService,
																			storeContext,
																			storeMappingService,
																			taxService,
																			workContext,
																			orderSettings,
																			paymentSettings,
																			rewardPointsSettings,
																			shippingSettings,
																			taxSettings)
		{
			_workContext = workContext;
            _shippingMethodService = shippingMethodService;
			_addressCityService = addressCityService;
			_paymentMethodAmountRestrictionService = paymentMethodAmountRestrictionService;
			_productService = productService;
			_roleRestrictionService = roleRestrictionService;
			_storeContext = storeContext;
			_rewardPointsSettings = rewardPointsSettings;
			_shoppingCartService = shoppingCartService;
			_orderTotalCalculationService = orderTotalCalculationService;
			_priceFormatter = priceFormatter;
			_rewardPointService = rewardPointService;
			_orderProcessingService = orderProcessingService;
			_paymentPluginManager = paymentPluginManager;
			_localizationService = localizationService;
			_paymentSettings = paymentSettings;
			_paymentService = paymentService;
			_taxService = taxService;
			_currencyService = currencyService;
			_genericAttributeService = genericAttributeService;
			_customerService = customerService;
			_paymentMethodRestrictionService = paymentMethodRestrictionService;
			_coreSettings = coreSettings;
			_paymentMethodShippingRestrictionService = paymentMethodShippingRestrictionService;
		}

		#endregion

		#region Methods

		public override async Task<CheckoutPaymentMethodModel> PreparePaymentMethodModelAsync(IList<ShoppingCartItem> cart, int filterByCountryId)
		{
			var model = new CheckoutPaymentMethodModel();

			var customer = await _workContext.GetCurrentCustomerAsync();
			var store = await _storeContext.GetCurrentStoreAsync();

			//reward points
			if (_rewardPointsSettings.Enabled && !await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
			{
				var shoppingCartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, true, false);
				var rewardPoint = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTotal.redeemedRewardPointsAmount, await _workContext.GetWorkingCurrencyAsync());
				if (shoppingCartTotal.redeemedRewardPoints > 0)
				{
					model.DisplayRewardPoints = true;
					model.RewardPointsToUseAmount = await _priceFormatter.FormatPriceAsync(rewardPoint, true, false);
					model.RewardPointsToUse = shoppingCartTotal.redeemedRewardPoints;
					model.RewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);

					//are points enough to pay for entire order? like if this option (to use them) was selected
					model.RewardPointsEnoughToPayForOrder = !await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, true);
				}
			}

			//filter by country
			var paymentMethods = await (await _paymentPluginManager
				.LoadActivePluginsAsync(customer, store.Id, filterByCountryId))
				.Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
				.WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart))
				.ToListAsync();

			paymentMethods = paymentMethods.OrderBy(o => o.PluginDescriptor.DisplayOrder).ToList();
			var shippingAddress = await _customerService.GetCustomerShippingAddressAsync(customer);

			var roles = await _customerService.GetCustomerRolesAsync(customer);
			var roleRestrictions = await _roleRestrictionService.GetPaymentMethodCustomerRoleRestrictionsByIdsAsync(roles.Select(a => a.Id).ToList());

			var basketAmount = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal.Value;
			var restrictedPaymentMethodsByAmount = await _paymentMethodAmountRestrictionService.GetAllAsync();

			var shippingRestrictions = await _paymentMethodShippingRestrictionService.GetAllAsync();
			var selectedShippingMethod = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);

			var doc = XDocument.Parse(selectedShippingMethod);
			var shippingName = doc.Root?.Element("Name")?.Value;

			int selectedShippingMethodId = 0;
			if (!string.IsNullOrEmpty(shippingName))
			{
				selectedShippingMethodId = (await _shippingMethodService.GetShippingMethodByNameAsync(shippingName))?.Id ?? 0;
			}

			foreach (var pm in paymentMethods)
			{
				if (roleRestrictions.Any(a => a.PaymentMethodSystemName == pm.PluginDescriptor.SystemName))
					continue;

				if (shippingRestrictions.Any(a => a.PaymentMethodSystemName == pm.PluginDescriptor.SystemName && a.ShippingMethodId == selectedShippingMethodId))
					continue;

				if (restrictedPaymentMethodsByAmount.Any())
				{
					var restrictedMethod = restrictedPaymentMethodsByAmount.FirstOrDefault(a => a.PaymentMethodSystemName == pm.PluginDescriptor.SystemName);

					if (restrictedMethod is not null)
					{
						if (restrictedMethod.MinAmount.HasValue && restrictedMethod.MinAmount.Value != 0 && restrictedMethod.MinAmount > basketAmount)
							continue;

						if (restrictedMethod.MaxAmount.HasValue && restrictedMethod.MaxAmount.Value != 0 && restrictedMethod.MaxAmount < basketAmount)
							continue;
					}
				}

				if (shippingAddress != null)
				{
					var cityId = (await _addressCityService.GetAddressCityByAddressIdAsync(shippingAddress.Id))?.CityId ?? 0;
					if (await _paymentMethodRestrictionService.CityRestrictionExistsAsync(pm.PluginDescriptor.SystemName, cityId)) continue;
				}

				if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
					continue;

				var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
				{
					Name = await _localizationService.GetLocalizedFriendlyNameAsync(pm, (await _workContext.GetWorkingLanguageAsync()).Id),
					Description = _paymentSettings.ShowPaymentMethodDescriptions ? await pm.GetPaymentMethodDescriptionAsync() : string.Empty,
					PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
					LogoUrl = await _paymentPluginManager.GetPluginLogoUrlAsync(pm)
				};
				//payment method additional fee
				var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart, pm.PluginDescriptor.SystemName);
				var (rateBase, _) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, customer);
				var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, await _workContext.GetWorkingCurrencyAsync());

				if (rate > decimal.Zero)
					pmModel.Fee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(rate, true);

				model.PaymentMethods.Add(pmModel);
			}

			//find a selected (previously) payment method
			var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
				NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
			if (!string.IsNullOrEmpty(selectedPaymentMethodSystemName))
			{
				var paymentMethodToSelect = model.PaymentMethods.ToList()
					.Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
				if (paymentMethodToSelect != null)
					paymentMethodToSelect.Selected = true;
			}
			//if no option has been selected, let's do it for the first one
			if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
			{
				var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
				if (paymentMethodToSelect != null)
					paymentMethodToSelect.Selected = true;
			}

			return model;

		}

		#endregion
	}
}
