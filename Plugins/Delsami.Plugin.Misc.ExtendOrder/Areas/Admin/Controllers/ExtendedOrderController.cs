using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Nop.Services.Messages;
using Nop.Services.Localization;
using Delsami.Plugin.Misc.ExtendedOrder.Infrastructure.Permissions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Misc.ExtendedOrder.Services;
using Nop.Services.Shipping;
using System.Linq;
using System.Collections.Generic;
using Nop.Services.Orders;
using Nop.Core.Events;
using Delsami.Plugin.Misc.Core.Events;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class ExtendedOrderController : BasePluginController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly ISubStatusService _subStatusService;
    private readonly IShippingService _shippingService;
    private readonly IOrderService _orderService;
    private readonly IEventPublisher _eventPublisher;

    #endregion

    #region Ctor

    public ExtendedOrderController(IPermissionService permissionService,
                                   ISettingService settingService,
                                   INotificationService notificationService,
                                   ILocalizationService localizationService,
                                   ISubStatusService subStatusService,
                                   IShippingService shippingService,
                                   IOrderService orderService,
                                   IEventPublisher eventPublisher)
    {
        _permissionService = permissionService;
        _settingService = settingService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _subStatusService = subStatusService;
        _shippingService = shippingService;
        _orderService = orderService;
        _eventPublisher = eventPublisher;
    }

    #endregion

    #region Methods

    public async Task<ActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
            return Content("Access denied");

        var extendedOrderSettings = await _settingService.LoadSettingAsync<ExtendedOrderSettings>();

        var configurationModel = new ConfigurationModel
        {
            OrderExpirationAsMinutes = extendedOrderSettings.OrderExpirationAsMinutes,
            SystemSubStatusId = extendedOrderSettings.systemSubStatusId,
            OrderCompleteAsDays = extendedOrderSettings.OrderCompleteAsDays,
            FulfillmentDecisionBaseUrl = extendedOrderSettings.FulfillmentDecisionBaseUrl,
            SelectedShippingMethods = !string.IsNullOrWhiteSpace(extendedOrderSettings.SelectedShippingMethods) ? extendedOrderSettings.SelectedShippingMethods.Split(",").Select(int.Parse).ToList() : new List<int>()
        };


        foreach (var substatus in await _subStatusService.GetSubStatusesAsync())
            configurationModel.AvailableSubStatuses.Add(new SelectListItem { Text = substatus.Name, Value = substatus.Id.ToString() });

        foreach (var shippingMethod in await _shippingService.GetAllShippingMethodsAsync())
            configurationModel.AvailableShippingMethods.Add(new SelectListItem { Text = shippingMethod.Name, Value = shippingMethod.Id.ToString() });

        return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/ExtendedOrder/Configure.cshtml", configurationModel);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
            return Content("Access denied");

        var extendedOrderSettings = await _settingService.LoadSettingAsync<ExtendedOrderSettings>();

        if (ModelState.IsValid)
        {

            extendedOrderSettings.OrderExpirationAsMinutes = model.OrderExpirationAsMinutes;
            extendedOrderSettings.systemSubStatusId = model.SystemSubStatusId;
            extendedOrderSettings.OrderCompleteAsDays = model.OrderCompleteAsDays;
            extendedOrderSettings.FulfillmentDecisionBaseUrl = model.FulfillmentDecisionBaseUrl;
            extendedOrderSettings.SelectedShippingMethods = string.Join(",", model.SelectedShippingMethods);


            _settingService.SaveSetting(extendedOrderSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Settings.Error"));

        return View("~/Plugins/Delsami.Misc.ExtendedOrder/Areas/Admin/Views/ExtendedOrder/Configure.cshtml", model);
    }

    public async Task<IActionResult> ManuallySendOrderToOutbox(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        var @event = new ManuallySendOrderToOutboxEvent(order);

        await _eventPublisher.PublishAsync(@event);

        return RedirectToAction("Configure");
    }
    #endregion
}
