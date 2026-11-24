using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Factories;
using ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using ZarIran.Plugin.Shipping.IRPost.Domain;
using ZarIran.Plugin.Shipping.IRPost.Infrastructures.Permissions;
using ZarIran.Plugin.Shipping.IRPost.Services;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Controllers
{
    public class PriceRangeController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IPriceRangeModelFactory _priceRangeModelFactory;
        private readonly IPriceRangeService _priceRangeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PriceRangeController(IPermissionService permissionService,
                                    IPriceRangeModelFactory priceRangeModelFactory,
                                    IPriceRangeService priceRangeService,
                                    ILocalizationService localizationService,
                                    INotificationService notificationService,
                                    IWorkContext workContext)
        {
            _permissionService = permissionService;
            _priceRangeModelFactory = priceRangeModelFactory;
            _priceRangeService = priceRangeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _workContext = workContext;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {

            if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
                return AccessDeniedView();

            //prepare model
            var model = _priceRangeModelFactory.PreparePriceRangeSearchModel(new PriceRangeSearchModel());

            return View("~/Plugins/ZarIran.Shipping.IRPost/Areas/Admin/Views/PriceRange/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PriceRangeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _priceRangeModelFactory.PreparePriceRangeListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
                return AccessDeniedView();

            //prepare model
            var model = _priceRangeModelFactory.PreparePriceRangeModel(new PriceRangeModel(), null);

            return View("~/Plugins/ZarIran.Shipping.IRPost/Areas/Admin/Views/PriceRange/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PriceRangeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
                return AccessDeniedView();

            var priceRange = model.ToEntity<PriceRange>();

            var validationResult = await _priceRangeService.IsValidAsync(priceRange);

            if (!validationResult.IsValid)
            {
                if (validationResult.Error == WeightRangeValidationError.MaxPriceInvalid)
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.MaxWeightInvalid"));

                if (validationResult.Error == WeightRangeValidationError.MinPriceInvalid)
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.MinWeightInvalid"));

                if (validationResult.Error == WeightRangeValidationError.MinAndMaxInvalid)
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.MinAndMaxWeightInvalid"));
            }

            if (ModelState.IsValid)
            {
                priceRange.ModifierCustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

                priceRange.StartDateUtc = DateTime.UtcNow;

                if (priceRange.ToUpperWeight)
                    priceRange.MaxWeight = null;

                await _priceRangeService.InsertAsync(priceRange);

                //_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = priceRange.Id });
            }

            //prepare model
            model = _priceRangeModelFactory.PreparePriceRangeModel(model, null);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/ZarIran.Shipping.IRPost/Areas/Admin/Views/PriceRange/Create.cshtml", model);
        }

        //public virtual async Task<IActionResult> Edit(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
        //        return AccessDeniedView();

        //    //try to get a PriceRange with the specified id
        //    var PriceRange = await _priceRangeService.GetPriceRangeAsync(id);
        //    if (PriceRange == null)
        //        return RedirectToAction("List");

        //    //prepare model
        //    var model = _priceRangeModelFactory.PreparePriceRangeModel(null, PriceRange);

        //    return View("~/Plugins/ZarIran.Shipping.IRPost/Areas/Admin/Views/PriceRange/Edit.cshtml", model);
        //}

        //[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        //public virtual async Task<IActionResult> Edit(PriceRangeModel model, bool continueEditing)
        //{
        //    if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
        //        return AccessDeniedView();

        //    //try to get a PriceRange with the specified id
        //    var priceRange = await _priceRangeService.GetPriceRangeAsync(model.Id);

        //    if (priceRange == null)
        //        return RedirectToAction("List");

        //    priceRange = model.ToEntity(priceRange);

        //    var validationResult = await _priceRangeService.IsValidAsync(priceRange);

        //    if (!validationResult.IsValid)
        //    {
        //        if (validationResult.Error == WeightRangeValidationError.MaxPriceInvalid)
        //            ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.MaxPriceInvalid"));

        //        if (validationResult.Error == WeightRangeValidationError.MinPriceInvalid)
        //            ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.MinPriceInvalid"));

        //        if (validationResult.Error == WeightRangeValidationError.MinAndMaxInvalid)
        //            ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.MinAndMaxPriceInvalid"));
        //    }


        //    if (ModelState.IsValid)
        //    {
        //        await _priceRangeService.UpdateAsync(priceRange);

        //        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.Updated"));

        //        if (!continueEditing)
        //            return RedirectToAction("List");

        //        return RedirectToAction("Edit", new { id = priceRange.Id });
        //    }

        //    //prepare model
        //    model = _priceRangeModelFactory.PreparePriceRangeModel(model, priceRange);

        //    //if we got this far, something failed, redisplay form
        //    return View("~/Plugins/ZarIran.Shipping.IRPost/Areas/Admin/Views/PriceRange/Edit.cshtml", model);
        //}

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
                return AccessDeniedView();


            //try to get a PriceRange with the specified id
            var priceRange = await _priceRangeService.GetPriceRangeAsync(id);

            if (priceRange == null)
                return new NullJsonResult();

            priceRange.ModifierCustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

            priceRange.EndDateUtc = DateTime.UtcNow;

            await _priceRangeService.DeleteAsync(priceRange);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRange.Deleted"));

            return new NullJsonResult();
        }


        #endregion
    }
}
