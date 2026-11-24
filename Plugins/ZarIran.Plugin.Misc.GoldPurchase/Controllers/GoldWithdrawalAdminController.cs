using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class GoldWithdrawalAdminController : BasePluginController
{
    private readonly IPermissionService _permissions;
    private readonly IWorkContext _work;
    private readonly GoldWithdrawalService _withdrawals;

    public GoldWithdrawalAdminController(IPermissionService permissions, IWorkContext work, GoldWithdrawalService withdrawals)
    {
        _permissions = permissions;
        _work = work;
        _withdrawals = withdrawals;
    }

    [HttpGet]
    public async Task<IActionResult> List(bool completed = false, int page = 1)
    {
        if (!await _permissions.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        ViewBag.Completed = completed;
        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Admin/Withdrawals.cshtml",
            await _withdrawals.ListAsync(completed, Math.Clamp(page, 1, 1000000)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (!await _permissions.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        var request = await _withdrawals.FindAsync(id);
        return request == null ? NotFound() :
            View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Admin/WithdrawalDetails.cshtml", request);
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id, bool paid)
    {
        if (!await _permissions.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        if (!paid)
            return RedirectToAction(nameof(Details), new { id });

        if (await _withdrawals.FindAsync(id) == null)
            return NotFound();

        await _withdrawals.CompleteAsync(id, (await _work.GetCurrentCustomerAsync()).Id);
        return RedirectToAction(nameof(List), new { completed = true });
    }
}
