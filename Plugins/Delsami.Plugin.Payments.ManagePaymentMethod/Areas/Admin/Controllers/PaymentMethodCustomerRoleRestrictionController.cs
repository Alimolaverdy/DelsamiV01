using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Delsami.Plugin.Payments.ManagePaymentMethod.Infrastructure.Permissions;
using Delsami.Plugin.Payments.ManagePaymentMethod.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class PaymentMethodCustomerRoleRestrictionController : BasePluginController
{
    #region Fields
    private readonly IPermissionService _permissionService;
    private readonly IDelsamiPaymentMethodModelFactory _DelsamiShippingModelFactory;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IPaymentMethodCustomerRoleRestrictionService _paymentMethodCustomerRoleRestrictionService;
    private readonly IPaymentPluginManager _paymentPluginManager;

    #endregion

    #region Ctor

    public PaymentMethodCustomerRoleRestrictionController(IPermissionService permissionService,
                                          IDelsamiPaymentMethodModelFactory DelsamiShippingModelFactory,
                                          INotificationService notificationService,
                                          ILocalizationService localizationService,
                                          IPaymentMethodCustomerRoleRestrictionService paymentMethodCustomerRoleRestrictionService,
                                          IPaymentPluginManager paymentPluginManager)
    {
        _permissionService = permissionService;
        _DelsamiShippingModelFactory = DelsamiShippingModelFactory;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _paymentMethodCustomerRoleRestrictionService = paymentMethodCustomerRoleRestrictionService;
        _paymentPluginManager = paymentPluginManager;
    }
    #endregion

    #region Methods

    public virtual async Task<IActionResult> Restrictions()
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return AccessDeniedView();

        //prepare model
        var model = await _DelsamiShippingModelFactory.PrepareRoleRestrictionModelAsync();

        return View("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethodCustomerRole/Restrictions.cshtml", model);
    }


    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("Restrictions")]
    public virtual async Task<IActionResult> RestrictionSave(DelsamiPaymentMethodCustomerRoleRestrictionModel model, IFormCollection form)
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return AccessDeniedView();

        await _paymentMethodCustomerRoleRestrictionService.DeleteAllCustomerRoleRestrictionAsync();


        var paymentMethods = await (await _paymentPluginManager.LoadActivePluginsAsync())
                                                               .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                                                               .ToListAsync();

        var paymentMethodRoleList = new List<PaymentMethodCustomerRoleRestriction>();

        foreach (var paymentMethod in paymentMethods)
        {
            var paymentMethodModel = paymentMethod.ToPluginModel<PaymentMethodModel>();

            var formKey = paymentMethodModel.SystemName;
            var CustomerRoleIdForRestriction = !StringValues.IsNullOrEmpty(form[formKey])
                ? form[formKey].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            CustomerRoleIdForRestriction.Select(a=> new PaymentMethodCustomerRoleRestriction { CustomerRoleId = a, PaymentMethodSystemName = paymentMethodModel.SystemName })
                                        .ToList()
                                        .ForEach(paymentMethodRoleList.Add);
        }

        await _paymentMethodCustomerRoleRestrictionService.InsertPaymentMethodCustomerRoleRestrictionAsync(paymentMethodRoleList);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

        return RedirectToAction("Restrictions");
    }
    #endregion

}
