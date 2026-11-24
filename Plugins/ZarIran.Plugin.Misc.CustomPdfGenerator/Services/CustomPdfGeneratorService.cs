using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.CustomPdfGenerator.Model;
using ZarIran.Plugin.Misc.CustomPdfGenerator.Templates;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Common.Pdf;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Models.Customer;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Services
{
    public class CustomPdfGeneratorService : PdfService, IPdfService
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IOrderShippingMethodCapacityMappingService _OrderShippingMethodService;
        private readonly IStoreService _storeService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly INopFileProvider _fileProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IOrderService _orderService;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IGiftCardService _giftCardService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly IVendorService _vendorService;
        private readonly IProductService _productService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly CustomPdfGeneratorSettings _customPdfGeneratorSettings;
        private readonly IShipmentService _shipmentService;
        private readonly IRepository<PictureBinary> _pictureBinaryRepo;
        private readonly IOrderInvoiceService _orderInvoiceService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly AddressSettings _addressSettings;
        private readonly IZarIranCustomerService _zarIranCustomerService;
        #endregion

        #region Ctor

        public CustomPdfGeneratorService(AddressSettings addressSettings,
                                         CatalogSettings catalogSettings,
                                         CurrencySettings currencySettings,
                                         IAddressAttributeFormatter addressAttributeFormatter,
                                         IAddressService addressService,
                                         IOrderShippingMethodCapacityMappingService orderShippingMethodService,
                                         ICountryService countryService,
                                         ICurrencyService currencyService,
                                         IDateTimeHelper dateTimeHelper,
                                         IGiftCardService giftCardService,
                                         IHtmlFormatter htmlFormatter,
                                         ILanguageService languageService,
                                         ILocalizationService localizationService,
                                         IMeasureService measureService,
                                         INopFileProvider fileProvider,
                                         IOrderService orderService,
                                         IPaymentPluginManager paymentPluginManager,
                                         IPaymentService paymentService,
                                         IPictureService pictureService,
                                         IPriceFormatter priceFormatter,
                                         IProductService productService,
                                         IRewardPointService rewardPointService,
                                         ISettingService settingService,
                                         IShipmentService shipmentService,
                                         IStateProvinceService stateProvinceService,
                                         IStoreContext storeContext,
                                         IStoreService storeService,
                                         IVendorService vendorService,
                                         IWorkContext workContext,
                                         MeasureSettings measureSettings,
                                         TaxSettings taxSettings,
                                         VendorSettings vendorSettings,
                                         ICustomerAttributeService customerAttributeService,
                                         ICustomerAttributeParser customerAttributeParser,
                                         ICustomerService customerService,
                                         CustomPdfGeneratorSettings customPdfGeneratorSettings,
                                         IRepository<PictureBinary> pictureBinaryRepo,
                                         IOrderInvoiceService orderInvoiceService,
                                         IZarIranCustomerService zarIranCustomerService) : base(addressSettings,
                                                                               catalogSettings,
                                                                               currencySettings,
                                                                               addressAttributeFormatter,
                                                                               addressService,
                                                                               countryService,
                                                                               currencyService,
                                                                               dateTimeHelper,
                                                                               giftCardService,
                                                                               htmlFormatter,
                                                                               languageService,
                                                                               localizationService,
                                                                               measureService,
                                                                               fileProvider,
                                                                               orderService,
                                                                               paymentPluginManager,
                                                                               paymentService,
                                                                               pictureService,
                                                                               priceFormatter,
                                                                               productService,
                                                                               rewardPointService,
                                                                               settingService,
                                                                               shipmentService,
                                                                               stateProvinceService,
                                                                               storeContext,
                                                                               storeService,
                                                                               vendorService,
                                                                               workContext,
                                                                               measureSettings,
                                                                               taxSettings,
                                                                               vendorSettings)
        {
            _storeContext = storeContext;
            _OrderShippingMethodService = orderShippingMethodService;
            _storeService = storeService;
            _languageService = languageService;
            _workContext = workContext;
            _settingService = settingService;
            _pictureService = pictureService;
            _fileProvider = fileProvider;
            _dateTimeHelper = dateTimeHelper;
            _orderService = orderService;
            _catalogSettings = catalogSettings;
            _vendorSettings = vendorSettings;
            _taxSettings = taxSettings;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _localizationService = localizationService;
            _giftCardService = giftCardService;
            _rewardPointService = rewardPointService;
            _customerAttributeService = customerAttributeService;
            _customerAttributeParser = customerAttributeParser;
            _customerService = customerService;
            _addressService = addressService;
            _vendorService = vendorService;
            _productService = productService;
            _htmlFormatter = htmlFormatter;
            _customPdfGeneratorSettings = customPdfGeneratorSettings;
            _shipmentService = shipmentService;
            _pictureBinaryRepo = pictureBinaryRepo;
            _orderInvoiceService = orderInvoiceService;
            _stateProvinceService = stateProvinceService;
            _addressSettings = addressSettings;
            _zarIranCustomerService = zarIranCustomerService;
        }

        #endregion

        #region Methods
        public override async Task PrintPackagingSlipToPdfAsync(Stream stream, Shipment shipment, Language language = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

            var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(order.StoreId);

            //language info
            language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

            if (language?.Published != true)
                language = await _workContext.GetWorkingLanguageAsync();

            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

            if (shipmentItems?.Any() != true)
                return;

            var orderItems = await shipmentItems
                .SelectAwait(async si => await _orderService.GetOrderItemByIdAsync(si.OrderItemId))
                .Where(pi => pi != null)
                .ToListAsync();

            if (orderItems?.Any() != true)
                return;

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            MemoryStream logoMemoryStream = null;

            if (_customPdfGeneratorSettings.LogoPictureId > 0)
            {
                var pictureBinary = await GetPictureBinaryByPictureIdAsync(_customPdfGeneratorSettings.LogoPictureId);

                logoMemoryStream = new MemoryStream(pictureBinary.BinaryData);
            }

            //if (order.ShippingAddressId == null || await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is not Address shippingAddress)
            //    throw new NopException($"Shipping is required, but address is not available. Order ID = {order.Id}");

            StateProvince stateProvince = null;

            var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);

            if (_addressSettings.StateProvinceEnabled && shippingAddress != null)
                stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId.Value);


            var shippingTime = await _OrderShippingMethodService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(order.Id);
            string deliveryTime = shippingTime is null ? string.Empty : $"\n{shippingTime.DeliveryDateOnUtc.ToShortDateString()} ({shippingTime.Time})";


            bool isDifferentUser = false;
            string reciverCustomerFullName = shippingAddress.FirstName + " " + shippingAddress.LastName;
            string orderCustomerFullName = customer.FirstName + " " + customer.LastName;


            if (!string.Equals(reciverCustomerFullName, orderCustomerFullName, StringComparison.InvariantCultureIgnoreCase))
            {
                orderCustomerFullName = $"{customer.FirstName} {customer.LastName} - {customer.Username}";
                isDifferentUser = true;
            }

            reciverCustomerFullName = $"{shippingAddress.FirstName} {shippingAddress.LastName} - {shippingAddress.PhoneNumber}";

            var source = new CustomShipmentSource
            {
                ShipmentNumberText = shipment.Id.ToString(),
                OrderNumberText = order.CustomOrderNumber,
                Address = await GetShippingAddressAsync(language, order),
                Products = await GetOrderVendorsProductItemsAsync(order, orderItems, language, shipmentItems),
                BracodeStream = GenerateBarcodeStream(shipment.Id),
                BracodeContent = GenerateBarcodeContent(shipment.Id),
                ZarIranPhoneNumber = _customPdfGeneratorSettings.PhoneNumber,
                OrderCustomerFullName = orderCustomerFullName,
                ReciverCustomerFullName = reciverCustomerFullName,
                LogoStream = logoMemoryStream,
                StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, language.Id) : "",
                CityName = shippingAddress?.City,
                DeliveryTime = deliveryTime,
                PostalCode = shippingAddress.ZipPostalCode,
                IsDifferentCustomer = isDifferentUser


            };

            var document = new PackagingSlipTemplate(source);

            document.GeneratePdf(stream);
        }

        public override async Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null, Vendor vendor = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //store info
            store ??= await _storeContext.GetCurrentStoreAsync();

            var orderStore = order.StoreId == 0 || order.StoreId == store?.Id ?
                store : await _storeService.GetStoreByIdAsync(order.StoreId);

            //language info
            language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

            if (language?.Published != true)
                language = await _workContext.GetWorkingLanguageAsync();

            //by default _pdfSettings contains settings for the current active store
            //and we need PdfSettings for the store which was used to place an order
            //so let's load it based on a store of the current order
            var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(orderStore.Id);

            byte[] logo = null;
            var logoPicture = await _pictureService.GetPictureByIdAsync(pdfSettingsByStore.LogoPictureId);
            if (logoPicture != null)
            {
                var logoFilePath = await _pictureService.GetThumbLocalPathAsync(logoPicture, 0, false);

                if (logoPicture.MimeType == MimeTypes.ImageSvg)
                {
                    logo = await _pictureService.ConvertSvgToPngAsync(logoFilePath);
                }
                else
                {
                    logo = await _fileProvider.ReadAllBytesAsync(logoFilePath);
                }
            }

            var date = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

            var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) ?
                new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn1
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2) ?
                new List<string>()
                : pdfSettingsByStore.InvoiceFooterTextColumn2
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            if (customer == null)
                throw new Exception(nameof(customer));

            var nationalCode = _zarIranCustomerService.GetNationalCode(customer.CustomCustomerAttributesXML);

            Address shippingAddress = null;

            if (order.ShippingAddressId.HasValue)
                shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);

            //a vendor should have access only to products
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

            var dividedOrderItems = await DivideOrderItemsAsync(orderItems);
            var orderInvoice = await _orderInvoiceService.GetOrderInvoiceByOrderIdAsync(order.Id);

            bool isProformaInvoice = true;
            if (order.OrderStatus == OrderStatus.Complete)
                isProformaInvoice = false;

            var shippingPrice = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);

            var source = new CustomOrderSource()
            {
                IsProformaInvoice = isProformaInvoice,
                StoreUrl = orderStore.Url?.Trim('/'),
                OrderDateUser = date,
                LogoData = logo,
                OrderNumberText = order.CustomOrderNumber,
                BillingAddress = await GetBillingAddressAsync(vendor, language, order),
                ShippingAddress = await GetShippingAddressAsync(language, order),
                VendorsProducts = (await PrepareProductItemsAsync(order, dividedOrderItems.VendorsOrderItems, language, true)).ToList(),
                ZarIranProducts = (await PrepareProductItemsAsync(order, dividedOrderItems.ZarIranOrderItems, language, false)).ToList(),
                ShowSkuInProductList = _catalogSettings.ShowSkuOnProductDetailsPage,
                ShowVendorInProductList = _vendorSettings.ShowVendorOnOrderDetailsPage,
                CheckoutAttributes = vendor is null ? order.CheckoutAttributeDescription : string.Empty, //vendors cannot see checkout attributes
                Totals = vendor is null ? await GetTotalsAsync(language, order) : new(), //vendors cannot see totals,
                VendorsTotals = await PrepareInvoiceTotalsAsync(language, order, dividedOrderItems.VendorsOrderItems),
                ZarIranTotals = await PrepareInvoiceTotalsAsync(language, order, dividedOrderItems.ZarIranOrderItems, true),
                OrderNotes = await GetOrderNotesAsync(pdfSettingsByStore, order, language),
                FooterTextColumn1 = column1Lines,
                FooterTextColumn2 = column2Lines,
                TotalQuantity = orderItems.Sum(s => s.Quantity).ToString(),
                CustomerNationalCode = nationalCode,
                CustomerZipPostalCode = shippingAddress?.ZipPostalCode,

                ZarIranProductsQuantity = dividedOrderItems.ZarIranOrderItems.Sum(s => s.Quantity),
                VendorsProductsQuantity = dividedOrderItems.VendorsOrderItems.Sum(s => s.Quantity),
                ShippingPrice = shippingPrice == 0 ? null : await _priceFormatter.FormatPriceAsync(shippingPrice,
                                                                      true,
                                                                      order.CustomerCurrencyCode,
                                                                      language.Id,
                                                                      false),
                InvoiceId = orderInvoice?.InvoiceId.ToString(),

                CompanyNameTitle = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.CompanyNameTitle"),
                CompanyAddressTitle = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.CompanyAddressTitle"),
                CompanyNationalCodeTitle = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.CompanyNationalCodeTitle"),
                CompanyPhoneNumberTitle = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.CompanyPhoneNumberTitle"),
                CompanyPostalCodeTitle = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.CompanyPostalCodeTitle"),
                CompanyRegisterNumberTitle = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.CompanyRegisterNumberTitle"),

                CompanyName = await _localizationService.GetResourceAsync("ZarIran.Company.Name"),
                CompanyAddress = await _localizationService.GetResourceAsync("ZarIran.Company.Address"),
                CompanyNationalCode = await _localizationService.GetResourceAsync("ZarIran.Company.NationalCode"),
                CompanyPhoneNumber = await _localizationService.GetResourceAsync("Sevenspikes.Themes.Traction.Common.PhoneNumber"),
                CompanyPostalCode = await _localizationService.GetResourceAsync("ZarIran.Company.PostalCode"),
                CompanyRegisterNumber = await _localizationService.GetResourceAsync("ZarIran.Company.RegisterNumber"),
            };


            var document = new OrderTemplate(source, _customPdfGeneratorSettings.ZarIranVendorId);

            document.GeneratePdf(stream);
        }

        public virtual async Task<IList<CustomerAttributeModel>> PrepareCustomCustomerAttributesAsync(Customer customer, string overrideAttributesXml = "")
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var result = new List<CustomerAttributeModel>();

            var customerAttributes = await _customerAttributeService.GetAllCustomerAttributesAsync();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _customerAttributeService.GetCustomerAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    customer.CustomCustomerAttributesXML;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                if (!_customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id).Any())
                                    break;

                                //clear default selection                                
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await _customerAttributeParser.ParseCustomerAttributeValuesAsync(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                result.Add(attributeModel);
            }

            return result;
        }

        public async Task<PictureBinary> GetPictureBinaryByPictureIdAsync(int pictureId)
        {
            var pictureBinary = await _pictureBinaryRepo.Table.FirstOrDefaultAsync(w => w.PictureId == pictureId);

            return pictureBinary;
        }

        #endregion

        #region Utilities

        private async Task<List<CustomProductItem>> PrepareProductItemsAsync(Order order,
                                                                             IList<OrderItem> orderItems,
                                                                             Language language,
                                                                             bool IsVendor,
                                                                             IList<ShipmentItem> shipmentItems = null)
        {
            var vendors = await _vendorService.GetVendorsByProductIdsAsync(orderItems.Select(item => item.ProductId).ToArray());

            Task<string> FormatPrice(decimal amount) => PreparePriceAsync(amount, true, order.CustomerCurrencyCode, language.Id, true, order.CurrencyRate);

            var result = new List<CustomProductItem>();

            foreach (var oi in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(oi.ProductId);

                if (IsVendor && product.VendorId == _customPdfGeneratorSettings.ZarIranVendorId)
                    continue;

                var productItem = new CustomProductItem
                {
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, language.Id),
                    VendorName = vendors.FirstOrDefault(v => v.Id == product.VendorId)?.Name ?? string.Empty,
                    VendorId = product.VendorId,
                    Quantity = shipmentItems is null
                                    ? oi.Quantity.ToString()
                                    : shipmentItems.FirstOrDefault(x => x.OrderItemId == oi.Id)?.Quantity.ToString(),

                    UnitPriceExcludeTax = await FormatPrice(oi.UnitPriceExclTax + oi.DiscountAmountExclTax / oi.Quantity),
                    UnitPriceIncludeTax = await FormatPrice(oi.UnitPriceInclTax),
                    PriceIncludeTax = await FormatPrice(oi.PriceInclTax),
                    PriceExcludeTax = await FormatPrice(oi.PriceExclTax),
                    Tax = await FormatPrice(oi.PriceInclTax - oi.PriceExclTax),
                    DiscountAmountExcludeTax = await FormatPrice(oi.DiscountAmountExclTax),
                };

                if (!string.IsNullOrEmpty(oi.AttributeDescription))
                {
                    var attributes = _htmlFormatter.ConvertHtmlToPlainText(oi.AttributeDescription, true, true);
                    productItem.ProductAttributes = attributes.Split('\n').ToList();
                }

                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                    productItem.Sku = await _productService.FormatSkuAsync(product, oi.AttributesXml);

                result.Add(productItem);
            }

            return result;
        }

        private async Task<List<ProductItem>> GetOrderVendorsProductItemsAsync(Order order, IList<OrderItem> orderItems, Language language, IList<ShipmentItem> shipmentItems = null)
        {

            var productItems = await PrepareProductItemsAsync(order, orderItems, language, true, shipmentItems);
            return productItems.Select(s => (ProductItem)s).ToList();
        }

        private MemoryStream GenerateBarcodeStream(int shipmentId)
        {

            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = _customPdfGeneratorSettings.BarcodeFormat == default ? BarcodeFormat.CODE_128 : _customPdfGeneratorSettings.BarcodeFormat,
                Options = new EncodingOptions
                {
                    Width = 135,
                    Height = 15,
                    PureBarcode = true,
                }
            };

            var pixelData = barcodeWriter.Write(GenerateBarcodeContent(shipmentId));

            var memoryStream = new MemoryStream();
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                for (int y = 0; y < pixelData.Height; y++)
                {
                    for (int x = 0; x < pixelData.Width; x++)
                    {
                        var index = (y * pixelData.Width + x) * 4;
                        var color = Color.FromArgb(pixelData.Pixels[index + 3], pixelData.Pixels[index + 2], pixelData.Pixels[index + 1], pixelData.Pixels[index + 0]);
                        bitmap.SetPixel(x, y, color);
                    }
                }

                bitmap.Save(memoryStream, ImageFormat.Png);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        protected override async Task<InvoiceTotals> GetTotalsAsync(Language lang, Order order)
        {
            var result = new InvoiceTotals();

            Task<string> FormatPrice(decimal amount) => PreparePriceAsync(amount, true, order.CustomerCurrencyCode, lang.Id, true, order.CurrencyRate);


            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax
                result.SubTotal = await FormatPrice(order.OrderSubtotalInclTax);
            }
            else
            {
                //excluding tax
                result.SubTotal = await FormatPrice(order.OrderSubtotalInclTax);

            }

            //discount (applied to order subtotal)
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
            {
                //order subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
                {
                    //including tax
                    result.SubTotal = await FormatPrice(order.OrderSubTotalDiscountInclTax);

                }
                else
                {
                    //excluding tax
                    result.SubTotal = await FormatPrice(order.OrderSubTotalDiscountExclTax);

                }
            }

            //shipping
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    result.SubTotal = await FormatPrice(order.OrderShippingInclTax);
                }
                else
                {
                    //excluding tax
                    result.SubTotal = await FormatPrice(order.OrderShippingExclTax);

                }
            }

            //payment fee
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
            {
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    result.SubTotal = await FormatPrice(order.PaymentMethodAdditionalFeeInclTax);

                }
                else
                {
                    //excluding tax
                    result.SubTotal = await FormatPrice(order.PaymentMethodAdditionalFeeExclTax);

                }
            }

            //tax
            var taxStr = string.Empty;
            var taxRates = new SortedDictionary<decimal, decimal>();
            bool displayTax;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = _orderService.ParseTaxRates(order, order.TaxRates);

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    taxStr = await FormatPrice(order.OrderTax);

                }
            }

            if (displayTax)
            {
                result.Tax = taxStr;
            }

            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    var taxRate = string.Format(await _localizationService.GetResourceAsync("Pdf.TaxRate", lang.Id),
                                                _priceFormatter.FormatTaxRate(item.Key));

                    var taxValue = await FormatPrice(item.Value);

                    result.TaxRates.Add($"{taxRate} {taxValue}");
                }
            }

            //discount (applied to order total)
            if (order.OrderDiscount > decimal.Zero)
            {
                result.Discount = await FormatPrice(order.OrderDiscount);

            }

            //gift cards
            foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
            {
                var gcTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.GiftCardInfo", lang.Id),
                    (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode);

                var gcAmountStr = await FormatPrice(gcuh.UsedValue);

                result.GiftCards.Add($"{gcTitle} {gcAmountStr}");
            }

            //reward points
            if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                var rpTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.RewardPoints", lang.Id),
                    -redeemedRewardPointsEntry.Points);

                var rpAmount = await FormatPrice(redeemedRewardPointsEntry.UsedAmount);

                result.RewardPoints = $"{rpTitle} {rpAmount}";
            }

            //order total
            result.OrderTotal = await FormatPrice(order.OrderTotal);

            return result;
        }

        private async Task<CustomInvoiceTotals> PrepareInvoiceTotalsAsync(Language lang, Order order, IList<OrderItem> orderItems, bool ZarIranProducts = false)
        {
            Task<string> FormatPrice(decimal amount) => PreparePriceAsync(amount, true, order.CustomerCurrencyCode, lang.Id, true, order.CurrencyRate);

            var vendors = await _vendorService.GetVendorsByProductIdsAsync(orderItems.Select(item => item.ProductId).ToArray());

            decimal unitPrice = 0;
            decimal discount = 0;
            decimal price = 0;
            decimal tax = 0;
            decimal total = 0;

            if (ZarIranProducts)
            {
                var productIds = (await _productService.GetProductsByIdsAsync(orderItems.Select(item => item.ProductId).ToArray())).Where(a => a.VendorId != _customPdfGeneratorSettings.ZarIranVendorId).Select(a => a.Id).ToList();
                orderItems = orderItems.Where(a => !productIds.Contains(a.ProductId)).ToList();
            }
            foreach (var oi in orderItems)
            {
                unitPrice += oi.UnitPriceExclTax;
                discount += oi.DiscountAmountExclTax;
                price += oi.PriceInclTax;
                tax += (oi.PriceInclTax - oi.PriceExclTax);
                total = +price;
            }


            var invoiceTotal = new CustomInvoiceTotals()
            {
                SubTotal = await FormatPrice(unitPrice),
                Discount = await FormatPrice(discount),
                Tax = await FormatPrice(tax),
                Shipping = await FormatPrice(order.OrderShippingExclTax),
                OrderTotalWithoutShipping = await FormatPrice(total),
                OrderTotal = await FormatPrice(total),

            };

            return invoiceTotal;
        }

        private string GenerateBarcodeContent(int shipmentId)
        {
            return $"S{shipmentId.ToString().PadLeft(8, '0')}";
        }

        private async Task<(IList<OrderItem> VendorsOrderItems, IList<OrderItem> ZarIranOrderItems)> DivideOrderItemsAsync(IList<OrderItem> orderItems)
        {
            List<OrderItem> vendorsOrderItems = new();
            List<OrderItem> ZarIranOrderItems = new();

            foreach (var oi in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(oi.ProductId);

                if (product.VendorId == _customPdfGeneratorSettings.ZarIranVendorId)
                {
                    ZarIranOrderItems.Add(oi);
                }
                else
                {
                    vendorsOrderItems.Add(oi);
                }
            }

            return (vendorsOrderItems, ZarIranOrderItems);
        }

        private async Task<string> PreparePriceAsync(decimal price, bool showCurrency, string currencyCode, int languageId, bool priceIncludesTax, decimal currencyRate)
        {

            var formatedPrice = _currencyService.ConvertCurrency(price, currencyRate);

            return await _priceFormatter.FormatPriceAsync(formatedPrice,
                                                          showCurrency,
                                                          currencyCode,
                                                          languageId,
                                                          priceIncludesTax);
        }
        #endregion
    }

}
