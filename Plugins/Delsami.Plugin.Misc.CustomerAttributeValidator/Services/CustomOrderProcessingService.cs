using DocumentFormat.OpenXml.EMMA;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Services
{
    public class CustomOrderProcessingService : OrderProcessingService
    {
        private readonly IWorkContext _workContext;
        private readonly IAffiliateService _affiliateService;
        private readonly TaxSettings _taxSettings;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public CustomOrderProcessingService(CurrencySettings currencySettings,
                                            IAddressService addressService,
                                            IAffiliateService affiliateService,
                                            ICheckoutAttributeFormatter checkoutAttributeFormatter,
                                            ICountryService countryService,
                                            ICurrencyService currencyService,
                                            ICustomerActivityService customerActivityService,
                                            ICustomerService customerService,
                                            ICustomNumberFormatter customNumberFormatter,
                                            IDiscountService discountService,
                                            IEncryptionService encryptionService,
                                            IEventPublisher eventPublisher,
                                            IGenericAttributeService genericAttributeService,
                                            IGiftCardService giftCardService,
                                            ILanguageService languageService,
                                            ILocalizationService localizationService,
                                            ILogger logger,
                                            IOrderService orderService,
                                            IOrderTotalCalculationService orderTotalCalculationService,
                                            IPaymentPluginManager paymentPluginManager,
                                            IPaymentService paymentService,
                                            IPdfService pdfService,
                                            IPriceCalculationService priceCalculationService,
                                            IPriceFormatter priceFormatter,
                                            IProductAttributeFormatter productAttributeFormatter,
                                            IProductAttributeParser productAttributeParser,
                                            IProductService productService,
                                            IReturnRequestService returnRequestService,
                                            IRewardPointService rewardPointService,
                                            IShipmentService shipmentService,
                                            IShippingService shippingService,
                                            IShoppingCartService shoppingCartService,
                                            IStateProvinceService stateProvinceService,
                                            IStoreService storeService,
                                            ITaxService taxService,
                                            IVendorService vendorService,
                                            IWebHelper webHelper,
                                            IWorkContext workContext,
                                            IWorkflowMessageService workflowMessageService,
                                            LocalizationSettings localizationSettings,
                                            OrderSettings orderSettings,
                                            PaymentSettings paymentSettings,
                                            RewardPointsSettings rewardPointsSettings,
                                            ShippingSettings shippingSettings,
                                            TaxSettings taxSettings,
                                            ICustomerAttributeValidationService customerAttributeValidationService,
                                            INotificationService notificationService) : base(currencySettings,
                                                                            addressService,
                                                                            affiliateService,
                                                                            checkoutAttributeFormatter,
                                                                            countryService,
                                                                            currencyService,
                                                                            customerActivityService,
                                                                            customerService,
                                                                            customNumberFormatter,
                                                                            discountService,
                                                                            encryptionService,
                                                                            eventPublisher,
                                                                            genericAttributeService,
                                                                            giftCardService,
                                                                            languageService,
                                                                            localizationService,
                                                                            logger,
                                                                            orderService,
                                                                            orderTotalCalculationService,
                                                                            paymentPluginManager,
                                                                            paymentService,
                                                                            pdfService,
                                                                            priceCalculationService,
                                                                            priceFormatter,
                                                                            productAttributeFormatter,
                                                                            productAttributeParser,
                                                                            productService,
                                                                            returnRequestService,
                                                                            rewardPointService,
                                                                            shipmentService,
                                                                            shippingService,
                                                                            shoppingCartService,
                                                                            stateProvinceService,
                                                                            storeService,
                                                                            taxService,
                                                                            vendorService,
                                                                            webHelper,
                                                                            workContext,
                                                                            workflowMessageService,
                                                                            localizationSettings,
                                                                            orderSettings,
                                                                            paymentSettings,
                                                                            rewardPointsSettings,
                                                                            shippingSettings,
                                                                            taxSettings)
        {
            _workContext = workContext;
            _affiliateService = affiliateService;
            _taxSettings = taxSettings;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _customerAttributeValidationService = customerAttributeValidationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        public override async Task<PlaceOrderResult> PlaceOrderAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new PlaceOrderResult();

            if (!await _customerAttributeValidationService.CurrentCustomerInfoCompletedAsync())
            {
                result.Errors.Add(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerInfoIsInCompelete"));

                return result;
            }
            result = await base.PlaceOrderAsync(processPaymentRequest);

            return result;
        }

        private async Task PrepareAndValidateRecurringShoppingAsync(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest)
        {
            var (recurringCyclesError, recurringCycleLength, recurringCyclePeriod, recurringTotalCycles) = await _shoppingCartService.GetRecurringCycleInfoAsync(details.Cart);

            if (!string.IsNullOrEmpty(recurringCyclesError))
                throw new NopException(recurringCyclesError);

            processPaymentRequest.RecurringCycleLength = recurringCycleLength;
            processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
            processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
        }
    }
}
