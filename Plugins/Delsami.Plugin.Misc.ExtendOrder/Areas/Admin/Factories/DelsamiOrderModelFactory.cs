using LinqToDB.Tools;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Delsami.Plugin.Misc.ExtendedOrder.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories
{
    public class DelsamiOrderModelFactory : IDelsamiOrderModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<OrderShippingMethod> _shippingMethodRepo;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingService;
        private readonly IExtendedOrderService _extendedOrderService;
        private readonly IShippingService _shippingService;
        private readonly AddressSettings _addressSettings;
        private readonly NopHttpClient _nopHttpClient;
        private readonly ISettingService _settingService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICountryService _countryService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;
        private readonly IStoreService _storeService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomerService _customerService;
        private readonly IPaymentService _paymentService;
        private readonly IAffiliateService _affiliateService;
        private readonly TaxSettings _taxSettings;
        private readonly IVendorService _vendorService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IGiftCardService _giftCardService;
        private readonly IEncryptionService _encryptionService;
        private readonly IDownloadService _downloadService;
        private readonly IDiscountService _discountService;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ISubStatusMappingService _subStatusMappingService;
        private readonly ISubStatusService _subStatusService;
        private readonly IDelsamiOrderReportService _DelsamiOrderReportService;

        #endregion

        #region Ctor
        public DelsamiOrderModelFactory(IWorkContext workContext,
                                         AddressSettings addressSettings,
                                         NopHttpClient nopHttpClient,
                                         IOrderShippingMethodCapacityMappingService orderShippingService,
                                         IExtendedOrderService extendedOrderService,
                                         ISettingService settingService,
                                         IRepository<OrderShippingMethod> shippingMethodRepo,
                                         IShippingService shippingService,
                                         IBaseAdminModelFactory baseAdminModelFactory,
                                         IPaymentPluginManager paymentPluginManager,
                                         ILocalizationService localizationService,
                                         ICountryService countryService,
                                         CatalogSettings catalogSettings,
                                         IDateTimeHelper dateTimeHelper,
                                         IProductService productService,
                                         IOrderService orderService,
                                         IAddressService addressService,
                                         IStoreService storeService,
                                         IPriceFormatter priceFormatter,
                                         IStateProvinceService stateProvinceService,
                                         ICustomerService customerService,
                                         IPaymentService paymentService,
                                         IAffiliateService affiliateService,
                                         TaxSettings taxSettings,
                                         IVendorService vendorService,
                                         IRewardPointService rewardPointService,
                                         IReturnRequestService returnRequestService,
                                         IPictureService pictureService,
                                         ICurrencyService currencyService,
                                         OrderSettings orderSettings,
                                         IOrderReportService orderReportService,
                                         IOrderProcessingService orderProcessingService,
                                         IGiftCardService giftCardService,
                                         IEncryptionService encryptionService,
                                         IDownloadService downloadService,
                                         IDiscountService discountService,
                                         CurrencySettings currencySettings,
                                         IAddressModelFactory addressModelFactory,
                                         ISubStatusMappingService subStatusMappingService,
                                         ISubStatusService subStatusService
,
                                         IDelsamiOrderReportService DelsamiOrderReportService)
        {
            _workContext = workContext;
            _shippingMethodRepo = shippingMethodRepo;
            _extendedOrderService = extendedOrderService;
            _orderShippingService = orderShippingService;
            _shippingService = shippingService;
            _addressSettings = addressSettings;
            _nopHttpClient = nopHttpClient;
            _settingService = settingService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _paymentPluginManager = paymentPluginManager;
            _localizationService = localizationService;
            _countryService = countryService;
            _catalogSettings = catalogSettings;
            _dateTimeHelper = dateTimeHelper;
            _productService = productService;
            _orderService = orderService;
            _addressService = addressService;
            _storeService = storeService;
            _priceFormatter = priceFormatter;
            _stateProvinceService = stateProvinceService;
            _customerService = customerService;
            _paymentService = paymentService;
            _affiliateService = affiliateService;
            _taxSettings = taxSettings;
            _vendorService = vendorService;
            _rewardPointService = rewardPointService;
            _returnRequestService = returnRequestService;
            _pictureService = pictureService;
            _currencyService = currencyService;
            _orderSettings = orderSettings;
            _orderReportService = orderReportService;
            _orderProcessingService = orderProcessingService;
            _giftCardService = giftCardService;
            _encryptionService = encryptionService;
            _downloadService = downloadService;
            _discountService = discountService;
            _currencySettings = currencySettings;
            _addressModelFactory = addressModelFactory;
            _subStatusMappingService = subStatusMappingService;
            _subStatusService = subStatusService;
            _DelsamiOrderReportService = DelsamiOrderReportService;
        }

        #endregion

        #region Methods
        public async Task<DelsamiOrderListModel> PrepareOrderListModelAsync(DelsamiOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get parameters to filter orders
            var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
            var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.VendorId = currentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
            var filterByProductId = product != null && (currentVendor == null || product.VendorId == currentVendor.Id)
                ? searchModel.ProductId : 0;

            //get orders
            var orders = await _extendedOrderService.SearchOrdersAsync(storeId: searchModel.StoreId,
                vendorId: searchModel.VendorId,
                productId: filterByProductId,
                warehouseId: searchModel.WarehouseId, ShippingMethodIds:new List<int> { searchModel.ShippingMethodId },
                paymentMethodSystemName: searchModel.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: searchModel.BillingPhone,
                billingEmail: searchModel.BillingEmail,
                billingLastName: searchModel.BillingLastName,
                billingCountryId: searchModel.BillingCountryId,
                orderNotes: searchModel.OrderNotes,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var shipments = (await _shippingMethodRepo.GetAllAsync(
                            query => query.Where(e => orders.Select(z => z.Id).Contains(e.OrderId)))).ToDictionary(a => a.OrderId, a => a);

            //prepare list model
            var model = await new DelsamiOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
            {
                //fill in model values from the entity
                return orders.SelectAwait(async order =>
                {

                    var shippingAddress = order.ShippingAddressId.HasValue ? await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) : (Address)null;

                    var stateProvince = (shippingAddress?.StateProvinceId.HasValue ?? false)
                        ? await _stateProvinceService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId.Value)
                        : null;

                    var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                    string customerRoleNames = "";

                    if (customer != null)
                    {
                        var customerRoles = await _customerService.GetCustomerRolesAsync(customer);
                        customerRoleNames = string.Join(", ", customerRoles.Select(r => r.Name));
                    }

                    var stateProvinceName = stateProvince?.Name ?? " ";
                    string deliveryTime = string.Empty;
                    if (shipments.TryGetValue(order.Id, out var shipping) && shipping.ShippingMethodId == 22)
                    {
                        var delivery = await _orderShippingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(order.Id);

                        if (delivery is not null)
                            deliveryTime = $"_{delivery.DeliveryDateOnUtc.ToLongDateString()}_{delivery.Time}";

                    }
                    var orderModel = new DelsamiOrderModel
                    {
                        Id = order.Id,
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatusId = order.PaymentStatusId,
                        ShippingStatusId = order.ShippingStatusId,
                        CustomerEmail = shippingAddress?.Email,
                        CustomerFullName = $"{customer.FirstName} {customer.LastName}",
                        CustomerRoles = customerRoleNames,
                        ShippingPersonFullName = $"{shippingAddress?.FirstName} {shippingAddress?.LastName}",
                        CustomerId = order.CustomerId,
                        CustomOrderNumber = order.CustomOrderNumber,
                        BriefAddress = $"{stateProvinceName} - {shippingAddress?.City}",
                        ShippingMethod = $"{order.ShippingMethod}{deliveryTime}",

                    };

                    //convert dates to the user time
                    orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                    orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                    orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                    orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                    return orderModel;
                });
            });

            return model;
        }

        public async Task<OrderSearchModel> PrepareOrderSearchModelAsync(DelsamiOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            searchModel.BillingPhoneEnabled = _addressSettings.PhoneEnabled;

            var licenseCheckModel = new LicenseCheckModel();
            try
            {
                var result = await _nopHttpClient.GetLicenseCheckDetailsAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
                    if (licenseCheckModel.DisplayWarning == false && licenseCheckModel.BlockPages == false)
                        await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", false);
                }
            }
            catch { }
            searchModel.LicenseCheckModel = licenseCheckModel;

            //prepare available order, payment and shipping statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            if (searchModel.AvailablePaymentStatuses.Any())
            {
                if (searchModel.PaymentStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
            }

            await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);
            if (searchModel.AvailableShippingStatuses.Any())
            {
                if (searchModel.ShippingStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.ShippingStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailableShippingStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailableShippingStatuses.FirstOrDefault().Selected = true;
            }

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

            //prepare available payment methods
            searchModel.AvailablePaymentMethods = (await _paymentPluginManager.LoadAllPluginsAsync()).Select(method =>
                new SelectListItem { Text = method.PluginDescriptor.FriendlyName, Value = method.PluginDescriptor.SystemName }).ToList();
            searchModel.AvailablePaymentMethods.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = string.Empty });

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare grid
            searchModel.SetGridPageSize();

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            var shippingMethods = (await _shippingService.GetAllShippingMethodsAsync()).Select(a => new SelectListItem()
            {
                Text = a.Name,
                Value = a.Id.ToString(),
            }).ToList();
            shippingMethods.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            searchModel.AvailableShippingMethod = shippingMethods;

            return searchModel;
        }

        public async Task<DelsamiOrderModel> PrepareOrderModelAsync(DelsamiOrderModel model, Order order, bool excludeProperties = false)
        {
            if (order != null)
            {
                //fill in model values from the entity
                model ??= new DelsamiOrderModel
                {
                    Id = order.Id,
                    OrderStatusId = order.OrderStatusId,
                    VatNumber = order.VatNumber,
                    CheckoutAttributeInfo = order.CheckoutAttributeDescription
                };

                model.BillingAddress = new AddressModel();
                model.ShippingAddress = new AddressModel();

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                model.OrderGuid = order.OrderGuid;
                model.CustomOrderNumber = order.CustomOrderNumber;
                model.CustomerIp = order.CustomerIp;
                model.CustomerId = customer.Id;
                model.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                model.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                model.CustomerInfo = await _customerService.IsRegisteredAsync(customer) ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);
                model.CustomValues = _paymentService.DeserializeCustomValues(order);

                var subStatusMapping = await _subStatusMappingService.GetLastOrderSubStatusAsync(order.Id);

                if (subStatusMapping != null)
                {
                    model.SubStatusId = subStatusMapping.SubStatusId;
                    model.SubStatus = (_subStatusService.GetOrderSubStatusAsync(subStatusMapping.SubStatusId).Result.Name);
                }

                var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);
                if (affiliate != null)
                {
                    model.AffiliateId = affiliate.Id;
                    model.AffiliateName = await _affiliateService.GetAffiliateFullNameAsync(affiliate);
                }

                //prepare order totals
                await PrepareOrderModelTotalsAsync(model, order);

                //prepare order items
                await PrepareOrderItemModelsAsync(model.Items, order);
                model.HasDownloadableProducts = model.Items.Any(item => item.IsDownload);

                //prepare payment info
                await PrepareOrderModelPaymentInfoAsync(model, order);

                //prepare shipping info
                await PrepareOrderModelShippingInfoAsync(model, order);

                //prepare nested search model
                PrepareOrderShipmentSearchModel(model.OrderShipmentSearchModel, order);
                PrepareOrderNoteSearchModel(model.OrderNoteSearchModel, order);
            }

            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;

            return model;
        }

        public virtual async Task<DelsamiOrderAggreratorModel> PrepareOrderAggregatorModelAsync(OrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (!_orderSettings.DisplayOrderSummary)
                return null;

            //get parameters to filter orders
            var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
            var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.VendorId = currentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
            var filterByProductId = product != null && (currentVendor == null || product.VendorId == currentVendor.Id)
                ? searchModel.ProductId : 0;

            //prepare additional model data
            var reportSummary = await _DelsamiOrderReportService.GetOrderAverageReportLineAsync(storeId: searchModel.StoreId,
                vendorId: searchModel.VendorId,
                productId: filterByProductId,
                warehouseId: searchModel.WarehouseId,
                paymentMethodSystemName: searchModel.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingPhone: searchModel.BillingPhone,
                billingEmail: searchModel.BillingEmail,
                billingLastName: searchModel.BillingLastName,
                billingCountryId: searchModel.BillingCountryId,
                orderNotes: searchModel.OrderNotes);

            var profit = await _orderReportService.ProfitReportAsync(storeId: searchModel.StoreId,
                vendorId: searchModel.VendorId,
                productId: filterByProductId,
                warehouseId: searchModel.WarehouseId,
                paymentMethodSystemName: searchModel.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingPhone: searchModel.BillingPhone,
                billingEmail: searchModel.BillingEmail,
                billingLastName: searchModel.BillingLastName,
                billingCountryId: searchModel.BillingCountryId,
                orderNotes: searchModel.OrderNotes);

            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            var shippingSum = await _priceFormatter
                .FormatShippingPriceAsync(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, false);
            var taxSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumTax, true, false);
            var totalSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumOrders, true, false);
            var profitSum = await _priceFormatter.FormatPriceAsync(profit, true, false);
            var subTotalExcludeTaxSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumOrderSubtotalExcludeTaxs, true, false);

            var model = new DelsamiOrderAggreratorModel
            {
                aggregatorprofit = profitSum,
                aggregatorshipping = shippingSum,
                aggregatortax = taxSum,
                aggregatortotal = totalSum,
                aggregatorsubtotalexcludetax = subTotalExcludeTaxSum,
            };

            return model;
        }

        #endregion

        #region Utilities

        protected virtual async Task PrepareOrderModelTotalsAsync(OrderModel model, Order order)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

            //subtotal
            model.OrderSubtotalInclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderSubtotalInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true);
            model.OrderSubtotalExclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderSubtotalExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false);
            model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
            model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;

            //discount (applied to order subtotal)
            var orderSubtotalDiscountInclTaxStr = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderSubTotalDiscountInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true);
            var orderSubtotalDiscountExclTaxStr = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderSubTotalDiscountExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false);
            if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
                model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
            model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
            model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

            //shipping
            model.OrderShippingInclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderShippingInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
            _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true,
                _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix);
            model.OrderShippingExclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderShippingExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false,
                _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix);
            model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
            model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

            //payment method additional fee
            if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = await _priceFormatter
                    .FormatOrderPriceAsync(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true,
                    _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix);
                model.PaymentMethodAdditionalFeeExclTax = await _priceFormatter
                    .FormatOrderPriceAsync(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false,
                _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix);
            }

            model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
            model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;

            //tax
            model.Tax = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
            var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
            var displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
            var displayTax = !displayTaxRates;
            foreach (var tr in taxRates)
            {
                model.TaxRates.Add(new OrderModel.TaxRate
                {
                    Rate = _priceFormatter.FormatTaxRate(tr.Key),
                    Value = await _priceFormatter
                        .FormatOrderPriceAsync(tr.Value, order.CurrencyRate, order.CustomerCurrencyCode,
                        _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false)
                });
            }

            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.TaxValue = order.OrderTax;
            model.TaxRatesValue = order.TaxRates;

            //discount
            if (order.OrderDiscount > 0)
            {
                model.OrderTotalDiscount = await _priceFormatter
                    .FormatOrderPriceAsync(-order.OrderDiscount, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
            }
            model.OrderTotalDiscountValue = order.OrderDiscount;

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                model.GiftCards.Add(new OrderModel.GiftCard
                {
                    CouponCode = (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId)).GiftCardCouponCode,
                    Amount = await _priceFormatter.FormatPriceAsync(-gcuh.UsedValue, true, false)
                });
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount =
                    await _priceFormatter.FormatPriceAsync(-redeemedRewardPointsEntry.UsedAmount, true, false);
            }

            //total
            model.OrderTotal = await _priceFormatter
                .FormatOrderPriceAsync(order.OrderTotal, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
            model.OrderTotalValue = order.OrderTotal;

            //refunded amount
            if (order.RefundedAmount > decimal.Zero)
                model.RefundedAmount = await _priceFormatter.FormatPriceAsync(order.RefundedAmount, true, false);

            //used discounts
            var duh = await _discountService.GetAllDiscountUsageHistoryAsync(orderId: order.Id);
            foreach (var d in duh)
            {
                var discount = await _discountService.GetDiscountByIdAsync(d.DiscountId);

                model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel
                {
                    DiscountId = d.DiscountId,
                    DiscountName = discount.Name
                });
            }

            //profit (hide for vendors)
            if (await _workContext.GetCurrentVendorAsync() != null)
                return;

            var profit = await _orderReportService.ProfitReportAsync(orderId: order.Id);
            model.Profit = await _priceFormatter
                .FormatOrderPriceAsync(profit, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
        }

        protected virtual async Task PrepareOrderItemModelsAsync(IList<OrderItemModel> models, Order order)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

            //get order items
            var vendor = await _workContext.GetCurrentVendorAsync();
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                //fill in model values from the entity
                var orderItemModel = new OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = product.Name,
                    Quantity = orderItem.Quantity,
                    IsDownload = product.IsDownload,
                    DownloadCount = orderItem.DownloadCount,
                    DownloadActivationType = product.DownloadActivationType,
                    IsDownloadActivated = orderItem.IsDownloadActivated,
                    UnitPriceInclTaxValue = orderItem.UnitPriceInclTax,
                    UnitPriceExclTaxValue = orderItem.UnitPriceExclTax,
                    DiscountInclTaxValue = orderItem.DiscountAmountInclTax,
                    DiscountExclTaxValue = orderItem.DiscountAmountExclTax,
                    SubTotalInclTaxValue = orderItem.PriceInclTax,
                    SubTotalExclTaxValue = orderItem.PriceExclTax,
                    AttributeInfo = orderItem.AttributeDescription
                };

                //fill in additional values (not existing in the entity)
                orderItemModel.Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
                orderItemModel.VendorName = (await _vendorService.GetVendorByIdAsync(product.VendorId))?.Name;

                //picture
                var orderItemPicture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
                (orderItemModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(orderItemPicture, 75);

                //license file
                if (orderItem.LicenseDownloadId.HasValue)
                {
                    orderItemModel.LicenseDownloadGuid = (await _downloadService
                        .GetDownloadByIdAsync(orderItem.LicenseDownloadId.Value))?.DownloadGuid ?? Guid.Empty;
                }

                var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

                //unit price
                orderItemModel.UnitPriceInclTax = await _priceFormatter
                    .FormatOrderPriceAsync(orderItem.UnitPriceInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
                orderItemModel.UnitPriceExclTax = await _priceFormatter
                    .FormatOrderPriceAsync(orderItem.UnitPriceExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);

                //discounts
                orderItemModel.DiscountInclTax = await _priceFormatter
                    .FormatOrderPriceAsync(orderItem.DiscountAmountInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
                orderItemModel.DiscountExclTax = await _priceFormatter
                    .FormatOrderPriceAsync(orderItem.DiscountAmountExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);

                //subtotal
                orderItemModel.SubTotalInclTax = await _priceFormatter
                    .FormatOrderPriceAsync(orderItem.PriceInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
                orderItemModel.SubTotalExclTax = await _priceFormatter
                    .FormatOrderPriceAsync(orderItem.PriceExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);

                //recurring info
                if (product.IsRecurring)
                {
                    orderItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Products.RecurringPeriod"),
                        product.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));
                }

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //prepare return request models
                await PrepareReturnRequestBriefModelsAsync(orderItemModel.ReturnRequests, orderItem);

                //gift card identifiers
                orderItemModel.PurchasedGiftCardIds = (await _giftCardService
                    .GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Select(card => card.Id).ToList();

                models.Add(orderItemModel);
            }
        }

        protected virtual async Task PrepareOrderModelPaymentInfoAsync(OrderModel model, Order order)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            //prepare billing address
            model.BillingAddress = billingAddress.ToModel(model.BillingAddress);

            model.BillingAddress.CountryName = (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name;
            model.BillingAddress.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress))?.Name;

            await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress, billingAddress);
            SetAddressFieldsAsRequired(model.BillingAddress);

            if (order.AllowStoringCreditCardNumber)
            {
                //card type
                model.CardType = _encryptionService.DecryptText(order.CardType);
                //cardholder name
                model.CardName = _encryptionService.DecryptText(order.CardName);
                //card number
                model.CardNumber = _encryptionService.DecryptText(order.CardNumber);
                //cvv
                model.CardCvv2 = _encryptionService.DecryptText(order.CardCvv2);
                //expiry date
                var cardExpirationMonthDecrypted = _encryptionService.DecryptText(order.CardExpirationMonth);
                if (!string.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                    model.CardExpirationMonth = cardExpirationMonthDecrypted;
                var cardExpirationYearDecrypted = _encryptionService.DecryptText(order.CardExpirationYear);
                if (!string.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                    model.CardExpirationYear = cardExpirationYearDecrypted;

                model.AllowStoringCreditCardNumber = true;
            }
            else
            {
                var maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(order.MaskedCreditCardNumber);
                if (!string.IsNullOrEmpty(maskedCreditCardNumberDecrypted))
                    model.CardNumber = maskedCreditCardNumberDecrypted;
            }

            //payment transaction info
            model.AuthorizationTransactionId = order.AuthorizationTransactionId;
            model.CaptureTransactionId = order.CaptureTransactionId;
            model.SubscriptionTransactionId = order.SubscriptionTransactionId;

            //payment method info
            var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
            model.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);

            //payment method buttons
            model.CanCancelOrder = _orderProcessingService.CanCancelOrder(order);
            model.CanCapture = await _orderProcessingService.CanCaptureAsync(order);
            model.CanMarkOrderAsPaid = _orderProcessingService.CanMarkOrderAsPaid(order);
            model.CanRefund = await _orderProcessingService.CanRefundAsync(order);
            model.CanRefundOffline = _orderProcessingService.CanRefundOffline(order);
            model.CanPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, decimal.Zero);
            model.CanPartiallyRefundOffline = _orderProcessingService.CanPartiallyRefundOffline(order, decimal.Zero);
            model.CanVoid = await _orderProcessingService.CanVoidAsync(order);
            model.CanVoidOffline = _orderProcessingService.CanVoidOffline(order);

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            model.MaxAmountToRefund = order.OrderTotal - order.RefundedAmount;

            //recurring payment record
            model.RecurringPaymentId = (await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id, showHidden: true)).FirstOrDefault()?.Id ?? 0;
        }


        protected virtual async Task PrepareOrderModelShippingInfoAsync(OrderModel model, Order order)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            model.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
            if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
                return;

            model.IsShippable = true;
            model.ShippingMethod = order.ShippingMethod;
            model.CanAddNewShipments = await _orderService.HasItemsToAddToShipmentAsync(order);
            model.PickupInStore = order.PickupInStore;
            if (!order.PickupInStore)
            {
                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
                var shippingCountry = await _countryService.GetCountryByAddressAsync(shippingAddress);

                model.ShippingAddress = shippingAddress.ToModel(model.ShippingAddress);
                model.ShippingAddress.CountryName = shippingCountry?.Name;
                model.ShippingAddress.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress))?.Name;
                await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress, shippingAddress);
                SetAddressFieldsAsRequired(model.ShippingAddress);
                model.ShippingAddressGoogleMapsUrl = "https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q=" +
                    $"{WebUtility.UrlEncode(shippingAddress.Address1 + " " + shippingAddress.ZipPostalCode + " " + shippingAddress.City + " " + (shippingCountry?.Name ?? string.Empty))}";
            }
            else
            {
                if (order.PickupAddressId is null)
                    return;

                var pickupAddress = await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value);

                var pickupCountry = await _countryService.GetCountryByAddressAsync(pickupAddress);

                model.PickupAddress = pickupAddress.ToModel(model.PickupAddress);
                model.PickupAddressGoogleMapsUrl = $"https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q=" +
                    $"{WebUtility.UrlEncode($"{pickupAddress.Address1} {pickupAddress.ZipPostalCode} {pickupAddress.City} {(pickupCountry?.Name ?? string.Empty)}")}";
            }
        }
        protected virtual OrderShipmentSearchModel PrepareOrderShipmentSearchModel(OrderShipmentSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            searchModel.OrderId = order.Id;

            //prepare nested search model
            PrepareShipmentItemSearchModel(searchModel.ShipmentItemSearchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        protected virtual ShipmentItemSearchModel PrepareShipmentItemSearchModel(ShipmentItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        protected virtual OrderNoteSearchModel PrepareOrderNoteSearchModel(OrderNoteSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            searchModel.OrderId = order.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        protected virtual async Task PrepareReturnRequestBriefModelsAsync(IList<OrderItemModel.ReturnRequestBriefModel> models, OrderItem orderItem)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(orderItemId: orderItem.Id);
            foreach (var returnRequest in returnRequests)
            {
                models.Add(new OrderItemModel.ReturnRequestBriefModel
                {
                    CustomNumber = returnRequest.CustomNumber,
                    Id = returnRequest.Id
                });
            }
        }
        protected virtual void SetAddressFieldsAsRequired(AddressModel model)
        {
            model.FirstNameRequired = true;
            model.LastNameRequired = true;
            model.EmailRequired = true;
            model.CompanyRequired = _addressSettings.CompanyRequired;
            model.CountyRequired = _addressSettings.CountyRequired;
            model.CityRequired = _addressSettings.CityRequired;
            model.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.PhoneRequired = _addressSettings.PhoneRequired;
            model.FaxRequired = _addressSettings.FaxRequired;
        }
        #endregion
    }
}
