using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Factories;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.WarehouseVendors;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Delsami.Plugin.Misc.ProductManagement.Services;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WarehouseVendorController : BasePluginController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IWarehouseVendorFactory _warehouseVendorFactory;
        private readonly IWarehouseVendorService _warehouseVendorService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public WarehouseVendorController(IWorkContext workContext,
                                         IPermissionService permissionService,
                                         IWarehouseVendorFactory warehouseVendorFactory,
                                         IWarehouseVendorService warehouseVendorService,
                                         INotificationService notificationService,
                                         ILocalizationService localizationService)
        {
            _workContext = workContext;
            _permissionService = permissionService;
            _warehouseVendorFactory = warehouseVendorFactory;
            _warehouseVendorService = warehouseVendorService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = await _warehouseVendorFactory.PrepareWarehouseVendorSearchModelAsync(new WarehouseVendorSearchModel());

            return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/WarehouseVendor/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(WarehouseVendorSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _warehouseVendorFactory.PrepareWarehouseVendorListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = await _warehouseVendorFactory.PrepareWarehouseVendorModelAsync(new WarehouseVendorModel(), null);

            return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/WarehouseVendor/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(WarehouseVendorModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (await _warehouseVendorService.IsExistVendorAsync(model.VendorId))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.VendorIsDuplicated"));

            if (await _warehouseVendorService.IsExistWarehouseAsync(model.WarehouseId))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseIsDuplicated"));

            if (ModelState.IsValid)
            {
                var warehouseVendor = model.ToEntity<WarehouseVendor>();

                await _warehouseVendorService.InsertAsync(warehouseVendor);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.WarehouseVendor.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = warehouseVendor.Id });
            }

            //prepare model
            model = await _warehouseVendorFactory.PrepareWarehouseVendorModelAsync(model, null);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/WarehouseVendor/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var warehouseVendor = await _warehouseVendorService.GetWarehouseVendorAsync(id);

            if (warehouseVendor == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _warehouseVendorFactory.PrepareWarehouseVendorModelAsync(null, warehouseVendor);

            return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/WarehouseVendor/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(WarehouseVendorModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var warehouseVendor = await _warehouseVendorService.GetWarehouseVendorAsync(model.Id);

            if (warehouseVendor == null)
                return RedirectToAction("List");

            if (await _warehouseVendorService.IsExistVendorAsync(model.VendorId, model.Id))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.VendorIsDuplicated"));

            if (await _warehouseVendorService.IsExistWarehouseAsync(model.WarehouseId, model.Id))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseIsDuplicated"));

            if (ModelState.IsValid)
            {
                warehouseVendor = model.ToEntity(warehouseVendor);

                await _warehouseVendorService.UpdateAsync(warehouseVendor);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.WarehouseVendor.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = warehouseVendor.Id });
            }

            //prepare model
            model = await _warehouseVendorFactory.PrepareWarehouseVendorModelAsync(model, warehouseVendor);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/WarehouseVendor/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var warehouseVendor = await _warehouseVendorService.GetWarehouseVendorAsync(id);

            if (warehouseVendor == null)
                return RedirectToAction("List");

            //delete a vendor
            await _warehouseVendorService.DeleteAsync(warehouseVendor);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.WarehouseVendor.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}
