using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Factories;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Domain;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CustomerAttributeValidationController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ICustomerAttributeValidationFactory _customerAttributeValidationFactory;
        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CustomerAttributeValidationController(IPermissionService permissionService,
            ICustomerAttributeValidationFactory customerAttributeValidationFactory,
            ICustomerAttributeValidationService customerAttributeValidationService,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _customerAttributeValidationFactory = customerAttributeValidationFactory;
            _customerAttributeValidationService = customerAttributeValidationService;
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

            var model = await _customerAttributeValidationFactory.PrepareCustomerAttributeValidationSearchModelAsync(new CustomerAttributeValidationSearchModel());

            return View("~/Plugins/Delsami.Misc.CustomerAttributeValidator/Areas/Admin/Views/CustomerAttributeValidation/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CustomerAttributeValidationSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var model = await _customerAttributeValidationFactory.PrepareCustomerAttributeValidationListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = await _customerAttributeValidationFactory.PrepareCustomerAttributeValidationModelAsync(new CustomerAttributeValidationModel(), null);

            return View("~/Plugins/Delsami.Misc.CustomerAttributeValidator/Areas/Admin/Views/CustomerAttributeValidation/Create.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(CustomerAttributeValidationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var customerAttributeValidationByCustomerAttribute = await _customerAttributeValidationService.GetCustomerAttributeValidationByCustomerAttributeIdAsync(model.CustomerAttributeId);

            if (customerAttributeValidationByCustomerAttribute != null)
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.CustomerAttributeDuplicated"));

            if (ModelState.IsValid)
            {

                var category = model.ToEntity<CustomerAttributeValidation>();

                await _customerAttributeValidationService.InsertAsync(category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Added"));

                return RedirectToAction("List");
            }

            model = await _customerAttributeValidationFactory.PrepareCustomerAttributeValidationModelAsync(model, null);

            return View("~/Plugins/Delsami.Misc.CustomerAttributeValidator/Areas/Admin/Views/CustomerAttributeValidation/Create.cshtml", model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var customerAttributeValidation = await _customerAttributeValidationService.GetByIdAsync(id);

            if (customerAttributeValidation == null)
                return RedirectToAction("List");

            var model = await _customerAttributeValidationFactory.PrepareCustomerAttributeValidationModelAsync(null, customerAttributeValidation);

            return View("~/Plugins/Delsami.Misc.CustomerAttributeValidator/Areas/Admin/Views/CustomerAttributeValidation/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit(CustomerAttributeValidationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var customerAttributeValidation = await _customerAttributeValidationService.GetByIdAsync(model.Id);

            if (customerAttributeValidation == null)
                return RedirectToAction("List");

            var customerAttributeValidationByCustomerAttribute = await _customerAttributeValidationService.GetCustomerAttributeValidationByCustomerAttributeIdAsync(model.CustomerAttributeId, model.Id);

            if (customerAttributeValidationByCustomerAttribute != null)
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.CustomerAttributeDuplicated"));

            if (ModelState.IsValid)
            {
                customerAttributeValidation = model.ToEntity(customerAttributeValidation);

                await _customerAttributeValidationService.UpdateAsync(customerAttributeValidation);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Updated"));

                return RedirectToAction("List");
            }

            model = await _customerAttributeValidationFactory.PrepareCustomerAttributeValidationModelAsync(model, customerAttributeValidation);

            return View("~/Plugins/Delsami.Misc.CustomerAttributeValidator/Areas/Admin/Views/CustomerAttributeValidation/Edit.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var customerAttributeValidation = await _customerAttributeValidationService.GetByIdAsync(id);

            if (customerAttributeValidation == null)
                return RedirectToAction("List");

            await _customerAttributeValidationService.DeleteAsync(customerAttributeValidation);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}
