using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using ZarIran.Plugin.Misc.ExtendedCustomer.Factories.CustomerShebaNumbers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Models.CustomerShebaNumbers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Services;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Controllers;
using System;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CustomerShebaNumberController : BasePluginController
    {
        #region Fields

        private readonly ICustomerShebaNumberService _customerShebaNumberService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ICustomerShebaNumberModelFactory _customerShebaNumberModelFactory;

        #endregion

        #region Ctor

        public CustomerShebaNumberController(ICustomerShebaNumberService customerShebaNumberService,
                                             ICustomerService customerService,
                                             IWorkContext workContext,
                                             ILocalizationService localizationService,
                                             INotificationService notificationService,
                                             ICustomerShebaNumberModelFactory customerShebaNumberModelFactory)
        {
            _customerShebaNumberService = customerShebaNumberService;
            _customerService = customerService;
            _workContext = workContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _customerShebaNumberModelFactory = customerShebaNumberModelFactory;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var searchModel = await _customerShebaNumberModelFactory.PrepareCustomerShebaNumberSearchModelAsync(new CustomerShebaNumberSearchModel());
            var model = await _customerShebaNumberModelFactory.PrepareShebaNumberListModelAsync(searchModel);

            return View("~/Plugins/ZarIran.Misc.ExtendedCustomer/Views/CustomerShebaNumber/List.cshtml", model);
        }

        public async Task<IActionResult> Create()
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var customerCar = _customerShebaNumberModelFactory.PrepareCustomerShebaNumberModel(null, null);

            return View("~/Plugins/ZarIran.Misc.ExtendedCustomer/Views/CustomerShebaNumber/Create.cshtml", customerCar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerShebaNumberModel model)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (ModelState.IsValid)
            {
                var customerShebaNumber = model.ToEntity<CustomerShebaNumber>();

                customerShebaNumber.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

                await _customerShebaNumberService.InsertAsync(customerShebaNumber);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Added"));

                return RedirectToAction("List");
            }

            var customerModel = _customerShebaNumberModelFactory.PrepareCustomerShebaNumberModel(model, null);

            return View("~/Plugins/ZarIran.Misc.ExtendedCustomer/Views/CustomerShebaNumber/Create.cshtml", customerModel);
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var customerShebaNumber = await _customerShebaNumberService.GetCustomerShebaNumberAsync(id);

            if (customerShebaNumber == null)
                return RedirectToAction("List");

            var customerShebaNumberModel = _customerShebaNumberModelFactory.PrepareCustomerShebaNumberModel(null, customerShebaNumber);

            return View("~/Plugins/ZarIran.Misc.ExtendedCustomer/Views/CustomerShebaNumber/Edit.cshtml", customerShebaNumberModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomerShebaNumberModel model)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var oldCustomerShebaNumber = await _customerShebaNumberService.GetCustomerShebaNumberAsync(model.Id);

            if (oldCustomerShebaNumber == null)
                throw new ArgumentNullException(nameof(oldCustomerShebaNumber));

            if (ModelState.IsValid)
            {
                var customerShebaNumber = model.ToEntity<CustomerShebaNumber>();

                customerShebaNumber.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

                await _customerShebaNumberService.UpdateAsync(customerShebaNumber);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Updated"));

                return RedirectToAction("List");
            }

            var customerShebaNumberModel = _customerShebaNumberModelFactory.PrepareCustomerShebaNumberModel(model, oldCustomerShebaNumber);

            return View("~/Plugins/ZarIran.Misc.ExtendedCustomer/Views/CustomerShebaNumber/Create.cshtml", customerShebaNumberModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();


            var customerShebaNumber = await _customerShebaNumberService.GetCustomerShebaNumberAsync(id);

            if (customerShebaNumber != null)
                await _customerShebaNumberService.DeleteAsync(customerShebaNumber);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Deleted"));

            return Json(new
            {
                redirect = Url.RouteUrl("CustomerShebaNumberList"),
            });
        }

        #endregion
    }
}
