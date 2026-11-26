using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.ExtendedOrder.Infrastructure.Permissions;
using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Delsami.Plugin.Misc.ExtendedOrder.Services;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class SubStatusController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISubStatusFactory _subStatusFactory;
        private readonly ISubStatusService _subStatusService;


        #endregion

        #region Ctor

        public SubStatusController(IPermissionService permissionService,
                                     ILocalizationService localizationService,
                                     INotificationService notificationService,
                                     ISubStatusFactory subStatusFactory,
                                     ISubStatusService subStatusService)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _subStatusFactory = subStatusFactory;
            _subStatusService = subStatusService;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
                return AccessDeniedView();

            //prepare model
            var model = _subStatusFactory.PrepareOrderSubStatusSearchModel(new OrderSubStatusSearchModel());

            return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/SubStatus/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(OrderSubStatusSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _subStatusFactory.PrepareOrderSubStatusListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
                return AccessDeniedView();

            //prepare model
            var model = _subStatusFactory.PrepareOrderSubStatusModel(new OrderSubStatusModel(), null);

            return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/SubStatus/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(OrderSubStatusModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
                return AccessDeniedView();


            if (ModelState.IsValid)
            {
                var orderSubStatusModel = model.ToEntity<OrderSubStatus>();

                await _subStatusService.InsertAsync(orderSubStatusModel);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.SubStatus.Add"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = orderSubStatusModel.Id });
            }

            //prepare model
            model = _subStatusFactory.PrepareOrderSubStatusModel(model, null);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/SubStatus/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a subStatus with the specified id
            var subStatus = await _subStatusService.GetOrderSubStatusAsync(id);
            if (subStatus == null)
                return RedirectToAction("List");

            //prepare model
            var model = _subStatusFactory.PrepareOrderSubStatusModel(null, subStatus);

            return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/SubStatus/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(OrderSubStatusModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //try to get a subStatus with the specified id
            var subStatus = await _subStatusService.GetOrderSubStatusAsync(model.Id);

            if (subStatus == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                subStatus = model.ToEntity(subStatus);

                await _subStatusService.UpdateAsync(subStatus);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = subStatus.Id });
            }

            //prepare model
            model = _subStatusFactory.PrepareOrderSubStatusModel(model, subStatus);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/SubStatus/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
                return AccessDeniedView();

            //try to get a OrderSubStatus with the specified id
            var orderSubStatus = await _subStatusService.GetOrderSubStatusAsync(id);

            if (orderSubStatus == null)
                return RedirectToAction("List");

            await _subStatusService.DeleteAsync(orderSubStatus);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}
