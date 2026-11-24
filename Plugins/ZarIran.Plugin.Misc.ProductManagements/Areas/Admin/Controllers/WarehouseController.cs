using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WarehouseController : BasePluginController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IShippingModelFactory _shippingModelFactory;
        private readonly IShippingService _shippingService;
        private readonly IAddressAttributeParser _addressAttributeParser;

        #endregion

        #region Ctor

        public WarehouseController(IAddressService addressService,
                                   ICustomerActivityService customerActivityService,
                                   ILocalizationService localizationService,
                                   INotificationService notificationService,
                                   IPermissionService permissionService,
                                   IShippingModelFactory shippingModelFactory,
                                   IShippingService shippingService,
                                   IAddressAttributeParser addressAttributeParser)
        {
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _shippingModelFactory = shippingModelFactory;
            _shippingService = shippingService;
            _addressAttributeParser = addressAttributeParser;
        }

        #endregion

        #region Methods

        //public virtual async Task<IActionResult> CreateWarehouse()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
        //        return AccessDeniedView();

        //    //prepare model
        //    var model = await _shippingModelFactory.PrepareWarehouseModelAsync(new WarehouseModel(), null);

        //    return View(model);
        //}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateWarehouse(WarehouseModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);

            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity<Address>();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                await _addressService.InsertAddressAsync(address);

                //fill entity from model
                var warehouse = model.ToEntity<Warehouse>();
                warehouse.AddressId = address.Id;

                await _shippingService.InsertWarehouseAsync(warehouse);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewWarehouse",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewWarehouse"), warehouse.Id), warehouse);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Added"));

                return continueEditing ? RedirectToAction("EditWarehouse", "Shipping", new { id = warehouse.Id }) : RedirectToAction("Warehouses", "Shipping");
            }

            //prepare model
            model = await _shippingModelFactory.PrepareWarehouseModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/ZarIran.Misc.ProductManagements/Areas/Admin/Views/Warehouse/CreateWarehouse.cshtml", model);
        }

        //public virtual async Task<IActionResult> EditWarehouse(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
        //        return AccessDeniedView();

        //    //try to get a warehouse with the specified id
        //    var warehouse = await _shippingService.GetWarehouseByIdAsync(id);
        //    if (warehouse == null)
        //        return RedirectToAction("Warehouses");

        //    //prepare model
        //    var model = await _shippingModelFactory.PrepareWarehouseModelAsync(null, warehouse);

        //    return View(model);
        //}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditWarehouse(WarehouseModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = await _shippingService.GetWarehouseByIdAsync(model.Id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            if (ModelState.IsValid)
            {
                var address = await _addressService.GetAddressByIdAsync(warehouse.AddressId) ??
                    new Address
                    {
                        CreatedOnUtc = DateTime.UtcNow
                    };
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;

                if (address.Id > 0)
                    await _addressService.UpdateAddressAsync(address);
                else
                    await _addressService.InsertAddressAsync(address);

                //fill entity from model
                warehouse = model.ToEntity(warehouse);

                warehouse.AddressId = address.Id;

                await _shippingService.UpdateWarehouseAsync(warehouse);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditWarehouse",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditWarehouse"), warehouse.Id), warehouse);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Updated"));

                return continueEditing ? RedirectToAction("EditWarehouse", "Shipping", warehouse.Id) : RedirectToAction("Warehouses", "Shipping");
            }

            //prepare model
            model = await _shippingModelFactory.PrepareWarehouseModelAsync(model, warehouse, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/ZarIran.Misc.ProductManagements/Areas/Admin/Views/Warehouse/EditWarehouse.cshtml", model);
        }

        #endregion
    }
}
