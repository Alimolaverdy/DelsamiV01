using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.PaymentMethodDiscounts;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Services;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class PaymentMethodDiscountController : BasePluginController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IPaymentMethodDiscountService _paymentMethodDiscountService;
        private readonly IPaymentMethodDiscountModelFactory _paymentMethodDiscountModelFactory;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        public PaymentMethodDiscountController(ICustomerActivityService customerActivityService,
            ICustomerRoleModelFactory customerRoleModelFactory,
            ICustomerService customerService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IPaymentMethodDiscountService paymentMethodDiscountService,
            IPaymentMethodDiscountModelFactory paymentMethodDiscountModelFactory,
            IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            AddressSettings addressSettings)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _workContext = workContext;
            _paymentMethodDiscountService = paymentMethodDiscountService;
            _paymentMethodDiscountModelFactory = paymentMethodDiscountModelFactory;
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _addressSettings = addressSettings;
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
            var model = await _paymentMethodDiscountModelFactory.PreparePaymentMethodDiscountSearchModelAsync(new PaymentMethodDiscountSearchModel());

            return View("~/Plugins/ZarIran.Payments.ManagePaymentMethod/Areas/Admin/Views/PaymentMethodDiscount/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(PaymentMethodDiscountSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _paymentMethodDiscountModelFactory.PreparePaymentMethodDiscountListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = await _paymentMethodDiscountModelFactory.PreparePaymentMethodDiscountModelAsync(new PaymentMethodDiscountModel(), null);

            return View("~/Plugins/ZarIran.Payments.ManagePaymentMethod/Areas/Admin/Views/PaymentMethodDiscount/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(PaymentMethodDiscountModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var invalidDiscountPercent = false;

            if (model.DiscountPercent < 0 || model.DiscountPercent > 100)
                invalidDiscountPercent = true;

            model.PaymentMethodSystemName = model.PaymentMethodSystemName.Trim();

            var paymentMethods = await (await _paymentPluginManager
                                                .LoadActivePluginsAsync())
                                                .ToListAsync();

            var availableSystemNames = paymentMethods.Select(s => s.PluginDescriptor.SystemName);
            var isExistSystemName = availableSystemNames.Contains(model.PaymentMethodSystemName);

            if (ModelState.IsValid && isExistSystemName && !invalidDiscountPercent)
            {
                var paymentMethodDiscount = model.ToEntity<PaymentMethodDiscount>();

                var activePaymentMethodDiscount = await _paymentMethodDiscountService.GetActiveAsync(model.PaymentMethodSystemName);

                if (activePaymentMethodDiscount != null)
                {
                    activePaymentMethodDiscount.EndDateTimeOnUtc = DateTime.UtcNow;

                    await _paymentMethodDiscountService.UpdateAsync(activePaymentMethodDiscount);
                }

                paymentMethodDiscount.StartDateTimeOnUtc = DateTime.UtcNow;
                await _paymentMethodDiscountService.InsertAsync(paymentMethodDiscount);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerRoles.Added"));

                return RedirectToAction("List");
            }

            if (!isExistSystemName)
                ModelState.AddModelError("PaymentMethodSystemName", await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.PaymentMethodSystemName.IsNotExist"));

            if (invalidDiscountPercent)
                ModelState.AddModelError("DiscountPercent", await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.DiscountPercent.Invalid"));

            //prepare model
            model = await _paymentMethodDiscountModelFactory.PreparePaymentMethodDiscountModelAsync(model, null);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/ZarIran.Payments.ManagePaymentMethod/Areas/Admin/Views/PaymentMethodDiscount/Create.cshtml", model);
        }

        //public virtual async Task<IActionResult> Edit(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
        //        return AccessDeniedView();

        //    var paymentMethodDiscount = await _paymentMethodDiscountService.GetPaymentMethodDiscountAsync(id);

        //    if (paymentMethodDiscount == null)
        //        return RedirectToAction("List");

        //    //prepare model
        //    var model = await _paymentMethodDiscountModelFactory.PreparePaymentMethodDiscountModelAsync(null, paymentMethodDiscount);

        //    return View("~/Plugins/ZarIran.Misc.CarInfo/Areas/Admin/Views/CarBrandModelRelation/Edit.cshtml", model);
        //}

        //[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        //public virtual async Task<IActionResult> Edit(PaymentMethodDiscountModel model, bool continueEditing)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
        //        return AccessDeniedView();

        //    //try to get a CarBrandModelRelation with the specified id
        //    var paymentMethodDiscount = await _paymentMethodDiscountService.GetPaymentMethodDiscountAsync(model.Id);

        //    if (paymentMethodDiscount == null)
        //        return RedirectToAction("List");

        //    //var isExist = await _carBrandModelRelationService.IsExistAsync(model.CarBrandSpecificationAttributeOptionId, model.ModelSpecificationAttributeOptionId, model.Id);

        //    //if (isExist)
        //    //    ModelState.AddModelError("", await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.CarBrandModelRelation.CarBrandModelRelationIsExist"));


        //    if (ModelState.IsValid)
        //    {

        //        paymentMethodDiscount = model.ToEntity(paymentMethodDiscount);

        //        await _paymentMethodDiscountService.UpdateAsync(paymentMethodDiscount);

        //        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

        //        if (!continueEditing)
        //            return RedirectToAction("List");

        //        return RedirectToAction("Edit", new { id = paymentMethodDiscount.Id });
        //    }

        //    //prepare model
        //    model = await _paymentMethodDiscountModelFactory.PreparePaymentMethodDiscountModelAsync(model, paymentMethodDiscount);

        //    //if we got this far, something failed, redisplay form
        //    return View("~/Plugins/ZarIran.Misc.CarInfo/Areas/Admin/Views/CarBrandModelRelation/Edit.cshtml", model);
        //}

        //[HttpPost]
        //public virtual async Task<IActionResult> End(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
        //        return AccessDeniedView();

        //    //try to get a CarBrandModelRelation with the specified id
        //    var paymentMethodDiscount = await _paymentMethodDiscountService.GetPaymentMethodDiscountAsync(id);

        //    if (paymentMethodDiscount == null)
        //        return RedirectToAction("List");

        //    await _paymentMethodDiscountService.EndAsync(paymentMethodDiscount);

        //    return RedirectToAction("List");
        //}

        #endregion
    }
}
