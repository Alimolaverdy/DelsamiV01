using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NobParts.Plugin.Misc.CustomerAttributeValidator.Services
{
    public class CustomShoppingCartService : ShoppingCartService
    {
        #region Fields

        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CustomShoppingCartService(CatalogSettings catalogSettings,
                                        IAclService aclService,
                                        IActionContextAccessor actionContextAccessor,
                                        ICheckoutAttributeParser checkoutAttributeParser,
                                        ICheckoutAttributeService checkoutAttributeService,
                                        ICurrencyService currencyService,
                                        ICustomerService customerService,
                                        IDateRangeService dateRangeService,
                                        IDateTimeHelper dateTimeHelper,
                                        IGenericAttributeService genericAttributeService,
                                        ILocalizationService localizationService,
                                        IPermissionService permissionService,
                                        IPriceCalculationService priceCalculationService,
                                        IPriceFormatter priceFormatter,
                                        IProductAttributeParser productAttributeParser,
                                        IProductAttributeService productAttributeService,
                                        IProductService productService,
                                        IRepository<ShoppingCartItem> sciRepository,
                                        IShippingService shippingService,
                                        IStaticCacheManager staticCacheManager,
                                        IStoreContext storeContext,
                                        IStoreService storeService,
                                        IStoreMappingService storeMappingService,
                                        IUrlHelperFactory urlHelperFactory,
                                        IUrlRecordService urlRecordService,
                                        IWorkContext workContext,
                                        OrderSettings orderSettings,
                                        ShoppingCartSettings shoppingCartSettings,
                                        ICustomerAttributeValidationService customerAttributeValidationService) : base(catalogSettings, aclService, actionContextAccessor, checkoutAttributeParser, checkoutAttributeService, currencyService, customerService, dateRangeService, dateTimeHelper, genericAttributeService, localizationService, permissionService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productService, sciRepository, shippingService, staticCacheManager, storeContext, storeService, storeMappingService, urlHelperFactory, urlRecordService, workContext, orderSettings, shoppingCartSettings)
        {
            _customerAttributeValidationService = customerAttributeValidationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public override async Task<IList<string>> AddToCartAsync(Customer customer, Product product, ShoppingCartType shoppingCartType, int storeId, string attributesXml = null, decimal customerEnteredPrice = 0, DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, int quantity = 1, bool addRequiredProducts = true)
        {
            var warnings = await base.AddToCartAsync(customer, product, shoppingCartType, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts);

            var invalidCustomerAttributes = await _customerAttributeValidationService.GetInvalidCustomerAttributesAsync(customer.CustomCustomerAttributesXML);
            var nullOrEmptyCustomerAttributes = await _customerAttributeValidationService.GetNullOrEmptyRequiredCustomerAttributesAsync(customer.CustomCustomerAttributesXML);

            foreach (var nullOrEmptyCustomerAttribute in nullOrEmptyCustomerAttributes)
            {
                var customerAttributeValidation = await _customerAttributeValidationService.GetCustomerAttributeValidationByCustomerAttributeIdAsync(nullOrEmptyCustomerAttribute.Id);

                if (customerAttributeValidation == null || !customerAttributeValidation.PreventFromShoppingInInvalidState)
                    continue;

                var warningMessage = string.Format(await _localizationService.GetResourceAsync("NobParts.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.HasNoValueInCustomerInfo"), nullOrEmptyCustomerAttribute.Name);

                warnings.Add(warningMessage);
            }


            foreach (var invalidCustomerAttribute in invalidCustomerAttributes)
            {
                var customerAttributeValidation = await _customerAttributeValidationService.GetCustomerAttributeValidationByCustomerAttributeIdAsync(invalidCustomerAttribute.Id);

                if (customerAttributeValidation == null || !customerAttributeValidation.PreventFromShoppingInInvalidState)
                    continue;

                var warningMessage = string.Format(await _localizationService.GetResourceAsync("NobParts.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.InvalidStateInCustomerInfo"), invalidCustomerAttribute.Name);

                warnings.Add(warningMessage);
            }

            return warnings;
        }

        public override async Task<IList<string>> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart, string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            var warnings = await base.GetShoppingCartWarningsAsync(shoppingCart, checkoutAttributesXml, validateCheckoutAttributes);

            var reuslt = await _customerAttributeValidationService.CurrentCustomerInfoCompletedAsync();

            if (!reuslt)
                warnings.Add(await _localizationService.GetResourceAsync("NobParts.Plugin.Misc.CustomerInfoIsInCompelete"));

            return warnings;
        }

        #endregion
    }
}
