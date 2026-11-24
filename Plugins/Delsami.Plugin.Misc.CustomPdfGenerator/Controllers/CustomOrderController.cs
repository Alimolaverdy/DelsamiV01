using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Controllers
{
    internal class CustomOrderController : OrderController
    {
        public CustomOrderController(IAddressAttributeParser addressAttributeParser,
                                     IAddressService addressService,
                                     ICustomerActivityService customerActivityService,
                                     ICustomerService customerService,
                                     IDateTimeHelper dateTimeHelper,
                                     IDownloadService downloadService,
                                     IEncryptionService encryptionService,
                                     IEventPublisher eventPublisher,
                                     IExportManager exportManager,
                                     IGiftCardService giftCardService,
                                     IImportManager importManager,
                                     ILocalizationService localizationService,
                                     INotificationService notificationService,
                                     IOrderModelFactory orderModelFactory,
                                     IOrderProcessingService orderProcessingService,
                                     IOrderService orderService,
                                     IPaymentService paymentService,
                                     IPdfService pdfService,
                                     IPermissionService permissionService,
                                     IPriceCalculationService priceCalculationService,
                                     IProductAttributeFormatter productAttributeFormatter,
                                     IProductAttributeParser productAttributeParser,
                                     IProductAttributeService productAttributeService,
                                     IProductService productService,
                                     IShipmentService shipmentService,
                                     IShippingService shippingService,
                                     IShoppingCartService shoppingCartService,
                                     IStoreContext storeContext,
                                     IWorkContext workContext,
                                     IWorkflowMessageService workflowMessageService,
                                     OrderSettings orderSettings) : base(addressAttributeParser, addressService, customerActivityService, customerService, dateTimeHelper, downloadService, encryptionService, eventPublisher, exportManager, giftCardService, importManager, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, permissionService, priceCalculationService, productAttributeFormatter, productAttributeParser, productAttributeService, productService, shipmentService, shippingService, shoppingCartService, storeContext, workContext, workflowMessageService, orderSettings)
        {
        }
    }
}
