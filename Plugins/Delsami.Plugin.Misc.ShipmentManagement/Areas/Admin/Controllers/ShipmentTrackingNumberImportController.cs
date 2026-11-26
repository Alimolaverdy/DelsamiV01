using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Delsami.Plugin.Misc.ShipmentManagement.Infrastructure.Permission;
using Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;
using Delsami.Plugin.Misc.ShipmentManagement.Services;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class ShipmentTrackingNumberImportController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IShipmentTrackingImportService _importService;
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;



        #endregion

        #region Ctor

        public ShipmentTrackingNumberImportController(IPermissionService permissionService,
                                                 IShipmentTrackingImportService importService,
                                                 IWorkContext workContext,
                                                 ILocalizationService localizationService,
                                                 ICustomerService customerService,
                                                 INopFileProvider fileProvider,
                                                 INotificationService notificationService)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _localizationService = localizationService;
            _customerService = customerService;
            _importService = importService;
            _notificationService = notificationService;
            _fileProvider = fileProvider;


        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Import()
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
                return AccessDeniedView();

            return View("~/Plugins/Delsami.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentTrackingNumberImport/Import.cshtml");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import(IFormFile uploadFile)
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
                return AccessDeniedView();

            if (uploadFile == null || uploadFile.Length == 0)
            {
                _notificationService.ErrorNotification("فایل انتخاب نشده");
                return View("~/Plugins/Delsami.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentTrackingNumberImport/Import.cshtml");
            }

            byte[] fileBytes;
            using (var stream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(stream);
                fileBytes = stream.ToArray();
            }

            var user = await _workContext.GetCurrentCustomerAsync();
            try
            {
                int importId = await _importService.ImportTrackingNumbersAsync(fileBytes, user.Id);
                _notificationService.SuccessNotification("ورود فایل با موفقیت انجام شد.");
                return RedirectToAction("Report", new { id = importId });
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex.Message);
                return View("~/Plugins/Delsami.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentTrackingNumberImport/Import.cshtml");
            }
        }

        public virtual async Task<IActionResult> ImportList()
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
                return AccessDeniedView();

            var model = new ImportSearchModel();

            model.SetGridPageSize();

            return View("~/Plugins/Delsami.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentTrackingNumberImport/ImportList.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportList(ImportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
                return await AccessDeniedDataTablesJson();

            var imports = await _importService.GetImportsAsync(searchModel);

            var model = new ShipmentImportListModel().PrepareToGrid(searchModel, imports, () =>
            {
                return imports.Select(import =>
                {

                    var importModel = new ShipmentImportModel
                    {
                        Id = import.Id,
                        CreatedOnUtc = import.CreatedOnUtc

                    };

                    return importModel;
                });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> Report(int id)
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
                return AccessDeniedView();

            var model = new ReportSearchModel { ShipmentImportId = id };
            model.SetGridPageSize();

            model.AvailableStatuses = await PrepareReportItemStatusesAsync();
            return View("~/Plugins/Delsami.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentTrackingNumberImport/Report.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Report(ReportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
                return await AccessDeniedDataTablesJson();

            var reports = await _importService.GetReportsByImportIdWithFilterAsync(searchModel);


            var model = await new ShipmentTrackNumberListModel().PrepareToGridAsync(searchModel, reports, () =>
            {
                return reports.SelectAwait(async report =>
                {

                    var importModel = new ImportedTrackNumberReportModel
                    {
                        OrderId = report.OrderId,
                        ShipmentId = report.ShipmentId,
                        CustomerName = GetFullName(await _customerService.GetCustomerByIdAsync(report.CustomerId)),
                        ErrorDescription = report.ErrorDescription,
                        Status = await _localizationService.GetLocalizedEnumAsync((ImportTrackNumberResult)report.Status),
                        StatusId = report.Status,
                        TrackingNumber = report.TrackingNumber,
                        CreatedOnUtc = report.CreatedOnUtc

                    };

                    return importModel;
                });
            });

            return Json(model);

        }


        public IActionResult DownloadSample()
        {
            var filePath = _fileProvider.MapPath("~/Plugins/Delsami.Misc.ShipmentManagement/Content/sample.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "sample.xlsx");
        }

        #endregion

        #region Private Method

        private string GetFullName(Customer customer)
        {
            if (customer == null)
                return "نامشخص";
            var firstName = customer.FirstName ?? "";
            var lastName = customer.LastName ?? "";
            var fullName = $"{firstName} {lastName}".Trim();
            return string.IsNullOrEmpty(fullName) ? "نامشخص" : fullName;
        }

        public virtual async Task<IList<SelectListItem>> PrepareReportItemStatusesAsync()
        {
            var items = new List<SelectListItem>();
            var availableStatusItems = await ImportTrackNumberResult.ShipmentIdIsNotValid.ToSelectListAsync(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            return items;
        }
        #endregion
    }
}
