using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.CustomPdfGenerator.Enums;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CustomOrderController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IPdfService _pdfService;
        private readonly IOrderService _orderService;
        private readonly CustomPdfGeneratorSettings _customPdfGeneratorSettings;
        #endregion

        #region Ctor

        public CustomOrderController(IPermissionService permissionService,
                                       IShipmentService shipmentService,
                                       IWorkContext workContext,
                                       OrderSettings orderSettings,
                                       IPdfService pdfService,
                                       IOrderService orderService,
                                       CustomPdfGeneratorSettings customPdfGeneratorSettings)
        {
            _permissionService = permissionService;
            _shipmentService = shipmentService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _pdfService = pdfService;
            _orderService = orderService;
            _customPdfGeneratorSettings = customPdfGeneratorSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> PdfPackagingSlipPrint(int shipmentId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintPackagingSlipToPdfAsync(stream, shipment, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());

                bytes = stream.ToArray();
            }

            using (var memoryStream = new MemoryStream(bytes))
            {
                PdfDocument pdfDocument = PdfReader.Open(memoryStream, PdfDocumentOpenMode.Modify);

                // Rotate each page by 90 degrees
                foreach (PdfPage page in pdfDocument.Pages)
                {
                    page.Rotate = (page.Rotate + GetRotateDegreeAsInt(_customPdfGeneratorSettings.RotateDegree)) % 360;
                }

                // Save the modified PDF to a new MemoryStream
                using (var outputStream = new MemoryStream())
                {
                    pdfDocument.Save(outputStream);
                    bytes = outputStream.ToArray(); // Return rotated PDF as byte array
                }
            }
            //stream.Position = 0;


            //Save the result document
            //pdfDoc.SaveToStream(stream);
            Response.Headers.Add("Content-Disposition", $"inline; filename=packagingslip_{shipment.Id}.pdf");

            return File(bytes, MimeTypes.ApplicationPdf);
        }

        public virtual async Task<IActionResult> PdfPackagingSlipDownload(int shipmentId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");


            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintPackagingSlipToPdfAsync(stream, shipment, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync());
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, $"packagingslip_{shipment.Id}.pdf");
        }

        public int GetRotateDegreeAsInt(RotateDegree rotateDegree)
        {
            return rotateDegree switch
            {
                RotateDegree.Zero => 0,
                RotateDegree.To90 => 90,
                RotateDegree.To180 => 180,
                RotateDegree.To270 => 270
            };
        }
        #endregion

        #region Utilities

        protected virtual async ValueTask<bool> HasAccessToShipmentAsync(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (await _workContext.GetCurrentVendorAsync() == null)
                //not a vendor; has access
                return true;

            return await HasAccessToOrderAsync(shipment.OrderId);
        }

        protected virtual async Task<bool> HasAccessToOrderAsync(int orderId)
        {
            if (orderId == 0)
                return false;

            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = currentVendor.Id;
            var hasVendorProducts = (await _orderService.GetOrderItemsAsync(orderId, vendorId: vendorId)).Any();

            return hasVendorProducts;
        }
        #endregion
    }
}