using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DateTimeHelper = ZarIran.Plugin.Misc.Core.Helpers.DateTimeHelper;
namespace ZarIran.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Factories
{

    public class ZarIranShoppingCartModelFactory : ShoppingCartModelFactory
    {
        #region Fields
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly ITaxService _taxService;
        private readonly IPriceFormatter _priceFormatter;


        #endregion

        #region Ctor
        public ZarIranShoppingCartModelFactory(CatalogSettings catalogSettings,
                                                IBaseAdminModelFactory baseAdminModelFactory,
                                                ICountryService countryService,
                                                ICustomerService customerService,
                                                IDateTimeHelper dateTimeHelper,
                                                ILocalizationService localizationService,
                                                IPriceFormatter priceFormatter,
                                                IProductAttributeFormatter productAttributeFormatter,
                                                IProductService productService,
                                                IShoppingCartService shoppingCartService,
                                                IStoreService storeService,
                                                ITaxService taxService) : base(catalogSettings, baseAdminModelFactory, countryService, customerService, dateTimeHelper, localizationService, priceFormatter, productAttributeFormatter, productService, shoppingCartService, storeService, taxService)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _localizationService = localizationService;
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _taxService = taxService;
            _priceFormatter = priceFormatter;

        }
        #endregion

        #region Methods

        public override async Task<ShoppingCartSearchModel> PrepareShoppingCartSearchModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available shopping cart types
            await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

            //set default search values
            searchModel.ShoppingCartType = ShoppingCartType.ShoppingCart;

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare nested search model
            PrepareShoppingCartItemSearchModel(searchModel.ShoppingCartItemSearchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public override async Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customers = await _customerService.GetCustomersWithShoppingCartsAsync(
                searchModel.ShoppingCartType,
                storeId: searchModel.StoreId,
                productId: searchModel.ProductId,
                createdFromUtc: searchModel.StartDate,
                createdToUtc: searchModel.EndDate,
                countryId: searchModel.BillingCountryId,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new ShoppingCartListModel();

            model = await model.PrepareToGridAsync(searchModel, customers, () =>
            {
                return customers.SelectAwait(async customer =>
                {
                    var extendedModel = new ZarIranShoppingCartModel
                    {
                        CustomerId = customer.Id,
                        CustomerUserName = (await _customerService.IsRegisteredAsync(customer))
                               ? customer.Username
                        : await _localizationService.GetResourceAsync("Admin.Customers.Guest")
                    };

                    extendedModel.TotalItems = (await _shoppingCartService.GetShoppingCartAsync(
                        customer, searchModel.ShoppingCartType,
                        searchModel.StoreId, searchModel.ProductId,
                        searchModel.StartDate, searchModel.EndDate))
                        .Sum(item => item.Quantity);

                    var shoppingCartItems = (await _shoppingCartService.GetShoppingCartAsync(customer)).Where(i => i.ShoppingCartTypeId == 1);
                    var customerRoles = (await _customerService.GetCustomerRolesAsync(customer)).Select(i => i?.Name).ToList();
                    var productIds = shoppingCartItems.Select(i => i.ProductId).ToArray();
                    var products = (await _productService.GetProductsByIdsAsync(productIds));

                    decimal totalAmount = 0;
                    decimal totalWithTax = 0;


                    foreach (var shoppingCartItem in shoppingCartItems)
                    {
                        var product = products.FirstOrDefault(p => p.Id == shoppingCartItem.ProductId);
                        if (product != null)
                        {
                            var itemTotal = product.Price * shoppingCartItem.Quantity;
                            totalAmount += itemTotal;

                            var taxed = await _taxService.GetProductPriceAsync(product, itemTotal);
                            totalWithTax += taxed.price;
                        }
                    }

                    extendedModel.CreatedOnUtc = DateTimeHelper.GetPersianStrFromDateTime(shoppingCartItems.OrderBy(o => o.CreatedOnUtc).FirstOrDefault()?.CreatedOnUtc.ToLocalTime(), true);
                    extendedModel.UpdatedOnUtc = DateTimeHelper.GetPersianStrFromDateTime(shoppingCartItems.OrderByDescending(o => o.UpdatedOnUtc).FirstOrDefault()?.UpdatedOnUtc.ToLocalTime(), true);
                    extendedModel.CustomerRoleNames = customerRoles;
                    extendedModel.TotalShoppingCartPrice = await _priceFormatter.FormatPriceAsync(totalWithTax);


                    return (ShoppingCartModel)extendedModel;
                });
            });

            return model;
        }
        #endregion
    }
}
