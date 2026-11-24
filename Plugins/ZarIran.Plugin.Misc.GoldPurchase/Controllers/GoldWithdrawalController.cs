using System;
using System.Linq;
using System.Data.Common;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using ZarIran.Plugin.Misc.ExtendedCustomer.Services;
using ZarIran.Plugin.Misc.GoldPurchase.Models;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Controllers;

[AutoValidateAntiforgeryToken]
public class GoldWithdrawalController : BasePluginController
{
    private readonly IWorkContext _work;
    private readonly IStoreContext _store;
    private readonly ICustomerService _customers;
    private readonly ICustomerShebaNumberService _shebas;
    private readonly GoldWithdrawalService _withdrawals;
    private readonly ILogger _logger;

    public GoldWithdrawalController(IWorkContext work, IStoreContext store, ICustomerService customers,
        ICustomerShebaNumberService shebas, GoldWithdrawalService withdrawals, ILogger logger)
    {
        _work = work;
        _store = store;
        _customers = customers;
        _shebas = shebas;
        _withdrawals = withdrawals;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var customer = await _work.GetCurrentCustomerAsync();
        if (!await _customers.IsRegisteredAsync(customer))
            return Challenge();

        var store = await _store.GetCurrentStoreAsync();
        var shebas = await _shebas.GetCustomerShebaNumbersAsync(customer.Id);
        var model = new GoldPayoutModel
        {
            Available = await _withdrawals.GetAvailableAsync(customer.Id, store.Id),
            Shebas = shebas.Select(item => new { item.Id, Number = GoldSheba.Normalize(item.Number) })
                .Where(item => item.Number != null).GroupBy(item => item.Number)
                .Select(group => new SelectListItem(group.Key, group.First().Id.ToString())).ToList(),
            Message = TempData["GoldPayout.Message"] as string,
            Error = TempData["GoldPayout.Error"] as string
        };
        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Withdrawal.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddSheba(string number, bool fromSell = false)
    {
        var customer = await _work.GetCurrentCustomerAsync();
        if (!await _customers.IsRegisteredAsync(customer))
            return Challenge();

        var normalized = GoldSheba.Normalize(number);
        if (normalized == null)
            TempData["GoldPayout.Error"] = "شماره شبا معتبر نیست. ۲۴ رقم بعد از IR را بررسی کنید.";
        else
        {
            var existing = await _shebas.GetCustomerShebaNumbersAsync(customer.Id);
            if (!existing.Any(item => GoldSheba.Normalize(item.Number) == normalized))
                await _shebas.InsertAsync(new CustomerShebaNumber { CustomerId = customer.Id, Number = normalized });
            TempData["GoldPayout.Message"] = "شماره شبا ذخیره شد.";
        }

        return RedirectToRoute(fromSell ? GoldPurchaseDefaults.SellRoute : GoldPurchaseDefaults.WithdrawalRoute);
    }

    [HttpPost]
    public async Task<IActionResult> RequestWithdrawal(Guid requestKey, decimal amount, int shebaId)
    {
        var customer = await _work.GetCurrentCustomerAsync();
        if (!await _customers.IsRegisteredAsync(customer))
            return Challenge();

        string error;
        try
        {
            error = ModelState.IsValid
                ? await _withdrawals.RequestAsync(customer, (await _store.GetCurrentStoreAsync()).Id, requestKey, amount, shebaId)
                : "اطلاعات درخواست برداشت معتبر نیست.";
        }
        catch (Exception exception) when (exception is DbException or TransactionException)
        {
            await _logger.ErrorAsync($"Could not commit gold withdrawal for customer {customer.Id}.", exception);
            error = "ثبت درخواست برداشت کامل نشد. دوباره تلاش کنید.";
        }

        if (error == null)
            TempData["GoldPayout.Message"] = "درخواست برداشت ثبت شد و مبلغ آن از موجودی قابل استفاده کنار گذاشته شد.";
        else
            TempData["GoldPayout.Error"] = error;

        return RedirectToRoute(GoldPurchaseDefaults.WithdrawalRoute);
    }
}
