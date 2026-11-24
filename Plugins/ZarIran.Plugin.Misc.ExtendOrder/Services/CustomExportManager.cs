using ClosedXML.Excel;
using ZarIran.Plugin.Misc.Core.Helpers;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.ExportImport.Help;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DateTimeHelper = ZarIran.Plugin.Misc.Core.Helpers.DateTimeHelper;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public class CustomExportManager : ExportManager
    {

        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IShipmentService _shipmentService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;

        #endregion

        #region Ctor

        public CustomExportManager(AddressSettings addressSettings,
                                   CatalogSettings catalogSettings,
                                   ICustomerActivityService customerActivityService,
                                   CustomerSettings customerSettings,
                                   DateTimeSettings dateTimeSettings,
                                   ForumSettings forumSettings,
                                   IAddressService addressService,
                                   ICategoryService categoryService,
                                   ICountryService countryService,
                                   ICurrencyService currencyService,
                                   ICustomerAttributeFormatter customerAttributeFormatter,
                                   ICustomerService customerService,
                                   IDateRangeService dateRangeService,
                                   IDateTimeHelper dateTimeHelper,
                                   IDiscountService discountService,
                                   IForumService forumService,
                                   IGdprService gdprService,
                                   IGenericAttributeService genericAttributeService,
                                   ILanguageService languageService,
                                   ILocalizationService localizationService,
                                   ILocalizedEntityService localizedEntityService,
                                   IManufacturerService manufacturerService,
                                   IMeasureService measureService,
                                   INewsLetterSubscriptionService newsLetterSubscriptionService,
                                   IOrderService orderService,
                                   IPictureService pictureService,
                                   IPriceFormatter priceFormatter,
                                   IProductAttributeService productAttributeService,
                                   IProductService productService,
                                   IProductTagService productTagService,
                                   IProductTemplateService productTemplateService,
                                   IShipmentService shipmentService,
                                   ISpecificationAttributeService specificationAttributeService,
                                   IStateProvinceService stateProvinceService,
                                   IStoreMappingService storeMappingService,
                                   IStoreService storeService,
                                   ITaxCategoryService taxCategoryService,
                                   IUrlRecordService urlRecordService,
                                   IVendorService vendorService,
                                   IWorkContext workContext,
                                   OrderSettings orderSettings,
                                   ProductEditorSettings productEditorSettings,
                                   IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService) : base(addressSettings,
                                                                                       catalogSettings,
                                                                                       customerActivityService,
                                                                                       customerSettings,
                                                                                       dateTimeSettings,
                                                                                       forumSettings,
                                                                                       addressService,
                                                                                       categoryService,
                                                                                       countryService,
                                                                                       currencyService,
                                                                                       customerAttributeFormatter,
                                                                                       customerService,
                                                                                       dateRangeService,
                                                                                       dateTimeHelper,
                                                                                       discountService,
                                                                                       forumService,
                                                                                       gdprService,
                                                                                       genericAttributeService,
                                                                                       languageService,
                                                                                       localizationService,
                                                                                       localizedEntityService,
                                                                                       manufacturerService,
                                                                                       measureService,
                                                                                       newsLetterSubscriptionService,
                                                                                       orderService,
                                                                                       pictureService,
                                                                                       priceFormatter,
                                                                                       productAttributeService,
                                                                                       productService,
                                                                                       productTagService,
                                                                                       productTemplateService,
                                                                                       shipmentService,
                                                                                       specificationAttributeService,
                                                                                       stateProvinceService,
                                                                                       storeMappingService,
                                                                                       storeService,
                                                                                       taxCategoryService,
                                                                                       urlRecordService,
                                                                                       vendorService,
                                                                                       workContext,
                                                                                       orderSettings,
                                                                                       productEditorSettings)
        {

            _catalogSettings = catalogSettings;
            _customerActivityService = customerActivityService;
            _addressService = addressService;
            _countryService = countryService;
            _customerService = customerService;
            _localizationService = localizationService;
            _stateProvinceService = stateProvinceService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _shipmentService = shipmentService;
            _productService = productService;
            _orderService = orderService;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
        }

        #endregion


        public override async Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders)
        {
            var ignore = await _workContext.GetCurrentVendorAsync() != null;

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

            //property array
            var properties = new[]
            {
                new PropertyByName<Order, Language>("OrderId", (p, l) => p.Id),
                new PropertyByName<Order, Language>("StoreId", (p, l) => p.StoreId),
                new PropertyByName<Order, Language>("OrderGuid", (p, l) => p.OrderGuid, ignore),
                new PropertyByName<Order, Language>("CustomerId", (p, l) => p.CustomerId, ignore),
                new PropertyByName<Order, Language>("CustomerGuid", async (p, l) => (await _customerService.GetCustomerByIdAsync(p.CustomerId))?.CustomerGuid, ignore),
                new PropertyByName<Order, Language>("OrderStatus", (p, l) => p.OrderStatusId, ignore)
                {
                    DropDownElements = await OrderStatus.Pending.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order, Language>("PaymentStatus", (p, l) => p.PaymentStatusId, ignore)
                {
                    DropDownElements = await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order, Language>("ShippingStatus", (p, l) => p.ShippingStatusId, ignore)
                {
                    DropDownElements = await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false)
                },
                new PropertyByName<Order, Language>("ShippingDate", async (p, l) =>
                {
                    if(p.ShippingRateComputationMethodSystemName!="ZarIran.Shipping.ZarIranLogistic")
                        return string.Empty;

                   var shippingMethodCapacity=await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(p.Id);
                    if(shippingMethodCapacity is null)
                        return "ارسال فوری";

                    return $"{DateTimeHelper.GetPersianStrFromDateTime(shippingMethodCapacity.DeliveryDateOnUtc.ToLocalTime())}";
                }),

                new PropertyByName<Order, Language>("ShippingTime", async (p, l) =>
                {
                    if(p.ShippingRateComputationMethodSystemName!="ZarIran.Shipping.ZarIranLogistic")
                        return string.Empty;

                   var shippingMethodCapacity=await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(p.Id);
                    if(shippingMethodCapacity is null)
                        return string.Empty;

                    return $"{shippingMethodCapacity.StartHour.Hours}-{shippingMethodCapacity.EndHour.Hours}";
                }),

                new PropertyByName<Order, Language>("DeliveryDate", async (p, l) =>
                {
                    if (p.ShippingStatusId != (int)ShippingStatus.Delivered)
                        return null;

                    var deliverydate=(await _shipmentService.GetShipmentsByOrderIdAsync(p.Id))
                        .FirstOrDefault()?.DeliveryDateUtc;

                    //return deliverydate?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.GetCultureInfo("en-US"));
                    return $"{DateTimeHelper.GetPersianStrFromDateTime(deliverydate?.ToLocalTime()??null)}";

                }),
                new PropertyByName<Order, Language>("DeliveryTime", async (p, l) =>
                {
                    if (p.ShippingStatusId != (int)ShippingStatus.Delivered)
                        return null;

                    var deliverydate=(await _shipmentService.GetShipmentsByOrderIdAsync(p.Id))
                        .FirstOrDefault()?.DeliveryDateUtc;

                    //return deliverydate?.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.GetCultureInfo("en-US"));
                    return $"{deliverydate?.ToLocalTime().ToString("HH:mm:ss", CultureInfo.GetCultureInfo("en-US"))}";

                }),
                new PropertyByName<Order, Language>("OrderSubtotalInclTax", (p, l) => p.OrderSubtotalInclTax, ignore),
                new PropertyByName<Order, Language>("OrderSubtotalExclTax", (p, l) => p.OrderSubtotalExclTax, ignore),
                new PropertyByName<Order, Language>("OrderSubTotalDiscountInclTax", (p, l) => p.OrderSubTotalDiscountInclTax, ignore),
                new PropertyByName<Order, Language>("OrderSubTotalDiscountExclTax", (p, l) => p.OrderSubTotalDiscountExclTax, ignore),
                new PropertyByName<Order, Language>("OrderShippingInclTax", (p, l) => p.OrderShippingInclTax, ignore),
                new PropertyByName<Order, Language>("OrderShippingExclTax", (p, l) => p.OrderShippingExclTax, ignore),
                new PropertyByName<Order, Language>("PaymentMethodAdditionalFeeInclTax", (p, l) => p.PaymentMethodAdditionalFeeInclTax, ignore),
                new PropertyByName<Order, Language>("PaymentMethodAdditionalFeeExclTax", (p, l) => p.PaymentMethodAdditionalFeeExclTax, ignore),
                new PropertyByName<Order, Language>("TaxRates", (p, l) => p.TaxRates, ignore),
                new PropertyByName<Order, Language>("OrderTax", (p, l) => p.OrderTax, ignore),
                new PropertyByName<Order, Language>("OrderTotal", (p, l) => p.OrderTotal, ignore),
                new PropertyByName<Order, Language>("RefundedAmount", (p, l) => p.RefundedAmount, ignore),
                new PropertyByName<Order, Language>("OrderDiscount", (p, l) => p.OrderDiscount, ignore),
                new PropertyByName<Order, Language>("CurrencyRate", (p, l) => p.CurrencyRate),
                new PropertyByName<Order, Language>("CustomerCurrencyCode", (p, l) => p.CustomerCurrencyCode),
                new PropertyByName<Order, Language>("AffiliateId", (p, l) => p.AffiliateId, ignore),
                new PropertyByName<Order, Language>("PaymentMethodSystemName", (p, l) => p.PaymentMethodSystemName, ignore),
                new PropertyByName<Order, Language>("ShippingPickupInStore", (p, l) => p.PickupInStore, ignore),
                new PropertyByName<Order, Language>("ShippingMethod", (p, l) => p.ShippingMethod),
                new PropertyByName<Order, Language>("ShippingRateComputationMethodSystemName", (p, l) => p.ShippingRateComputationMethodSystemName, ignore),
                new PropertyByName<Order, Language>("CustomValuesXml", (p, l) => p.CustomValuesXml, ignore),
                new PropertyByName<Order, Language>("VatNumber", (p, l) => p.VatNumber, ignore),
                //new PropertyByName<Order, Language>("CreatedOn", (p, l) => p.CreatedOnUtc.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.GetCultureInfo("en-US"))),
                new PropertyByName<Order, Language>("CreatedDate", (p, l) =>DateTimeHelper.GetPersianStrFromDateTime( p.CreatedOnUtc.ToLocalTime())),
                new PropertyByName<Order, Language>("CreatedTime", (p, l) => p.CreatedOnUtc.ToLocalTime().ToString("HH:mm:ss", CultureInfo.GetCultureInfo("en-US"))),
                new PropertyByName<Order, Language>("BillingFirstName", async (p, l) => (await orderBillingAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("BillingLastName", async (p, l) => (await orderBillingAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("BillingEmail", async (p, l) => (await orderBillingAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCompany", async (p, l) => (await orderBillingAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCountry", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCountryCode", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderBillingAddress(p)))?.TwoLetterIsoCode, ignore),
                new PropertyByName<Order, Language>("BillingStateProvince", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("BillingStateProvinceAbbreviation", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderBillingAddress(p)))?.Abbreviation, ignore),
                new PropertyByName<Order, Language>("BillingCounty", async (p, l) => (await orderBillingAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order, Language>("BillingCity", async (p, l) => (await orderBillingAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order, Language>("BillingAddress1", async (p, l) => (await orderBillingAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order, Language>("BillingAddress2", async (p, l) => (await orderBillingAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order, Language>("BillingZipPostalCode", async (p, l) => (await orderBillingAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order, Language>("BillingPhoneNumber", async (p, l) => (await orderBillingAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order, Language>("BillingFaxNumber", async (p, l) => (await orderBillingAddress(p))?.FaxNumber ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingFirstName", async (p, l) => (await orderAddress(p))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingLastName", async (p, l) => (await orderAddress(p))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingEmail", async (p, l) => (await orderAddress(p))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCompany", async (p, l) => (await orderAddress(p))?.Company ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCountry", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCountryCode", async (p, l) => (await _countryService.GetCountryByAddressAsync(await orderAddress(p)))?.TwoLetterIsoCode, ignore),
                new PropertyByName<Order, Language>("ShippingStateProvince", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingStateProvinceAbbreviation", async (p, l) => (await _stateProvinceService.GetStateProvinceByAddressAsync(await orderAddress(p)))?.Abbreviation, ignore),
                new PropertyByName<Order, Language>("ShippingCounty", async (p, l) => (await orderAddress(p))?.County ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingCity", async (p, l) => (await orderAddress(p))?.City ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingAddress1", async (p, l) => (await orderAddress(p))?.Address1 ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingAddress2", async (p, l) => (await orderAddress(p))?.Address2 ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingZipPostalCode", async (p, l) => (await orderAddress(p))?.ZipPostalCode ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingPhoneNumber", async (p, l) => (await orderAddress(p))?.PhoneNumber ?? string.Empty),
                new PropertyByName<Order, Language>("ShippingFaxNumber", async (p, l) => (await orderAddress(p))?.FaxNumber ?? string.Empty),
                new PropertyByName<Order, Language>("Weight", async (p, l) => (await _orderService.GetOrderItemsAsync(p.Id))?.Sum(s=>s.ItemWeight*s.Quantity), ignore),
            };

            //activity log
            await _customerActivityService.InsertActivityAsync("ExportOrders",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ExportOrders"), orders.Count));

            return _orderSettings.ExportWithProducts
                ? await ExportOrderToXlsxWithProductsAsync(properties, orders)
                : await new PropertyManager<Order, Language>(properties, _catalogSettings).ExportToXlsxAsync(orders);
        }

        private async Task<byte[]> ExportOrderToXlsxWithProductsAsync(PropertyByName<Order, Language>[] properties, IEnumerable<Order> itemsToExport)
        {
            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem, Language>("OrderItemGuid", (oi, l) => oi.OrderItemGuid),
                new PropertyByName<OrderItem, Language>("Name", async (oi, l) => (await _productService.GetProductByIdAsync(oi.ProductId)).Name),
                new PropertyByName<OrderItem, Language>("Sku", async (oi, l) => await _productService.FormatSkuAsync(await _productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml)),
                new PropertyByName<OrderItem, Language>("PriceExclTax", (oi, l) => oi.UnitPriceExclTax),
                new PropertyByName<OrderItem, Language>("PriceInclTax", (oi, l) => oi.UnitPriceInclTax),
                new PropertyByName<OrderItem, Language>("Quantity", (oi, l) => oi.Quantity),
                new PropertyByName<OrderItem, Language>("DiscountExclTax", (oi, l) => oi.DiscountAmountExclTax),
                new PropertyByName<OrderItem, Language>("DiscountInclTax", (oi, l) => oi.DiscountAmountInclTax),
                new PropertyByName<OrderItem, Language>("TotalExclTax", (oi, l) => oi.PriceExclTax),
                new PropertyByName<OrderItem, Language>("TotalInclTax", (oi, l) => oi.PriceInclTax)
            };

            var orderItemsManager = new PropertyManager<OrderItem, Language>(orderItemProperties, _catalogSettings);

            await using var stream = new MemoryStream();
            // ok, we can run the real code of the sample now
            using (var workbook = new XLWorkbook())
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handles to the worksheets
                // Worksheet names cannot be more than 31 characters
                var worksheet = workbook.Worksheets.Add(typeof(Order).Name);
                var fpWorksheet = workbook.Worksheets.Add("DataForProductsFilters");
                fpWorksheet.Visibility = XLWorksheetVisibility.VeryHidden;

                //create Headers and format them 
                var manager = new PropertyManager<Order, Language>(properties, _catalogSettings);
                manager.WriteDefaultCaption(worksheet);

                var row = 2;
                foreach (var order in itemsToExport)
                {
                    manager.CurrentObject = order;
                    await manager.WriteDefaultToXlsxAsync(worksheet, row++);

                    //a vendor should have access only to his products
                    var vendor = await _workContext.GetCurrentVendorAsync();
                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

                    if (!orderItems.Any())
                        continue;

                    orderItemsManager.WriteDefaultCaption(worksheet, row, 2);
                    worksheet.Row(row).OutlineLevel = 1;
                    worksheet.Row(row).Collapse();

                    foreach (var orderItem in orderItems)
                    {
                        row++;
                        orderItemsManager.CurrentObject = orderItem;
                        await orderItemsManager.WriteDefaultToXlsxAsync(worksheet, row, 2, fpWorksheet);
                        worksheet.Row(row).OutlineLevel = 1;
                        worksheet.Row(row).Collapse();
                    }

                    row++;
                }

                workbook.SaveAs(stream);
            }

            return stream.ToArray();
        }
    }
}
