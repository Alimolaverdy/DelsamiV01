using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Globalization;
using System;
using ZarIran.Plugin.Misc.GoldPurchase.Models;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Controllers;

/// <summary>
/// Backs the "Gold wallet" card on the admin customer page (Admin/GoldWalletAdmin/Adjust).
/// The card cannot contain its own &lt;form&gt; (the customer page already is one), so it posts via AJAX,
/// exactly like nopCommerce's own reward-points card does.
/// </summary>
[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class GoldWalletAdminController : BasePluginController
{
    private readonly IPermissionService _permissionService;
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    private readonly IGoldWalletService _goldWalletService;

    public GoldWalletAdminController(
        IPermissionService permissionService,
        ICustomerService customerService,
        IWorkContext workContext,
        IGoldWalletService goldWalletService)
    {
        _permissionService = permissionService;
        _customerService = customerService;
        _workContext = workContext;
        _goldWalletService = goldWalletService;
    }

    [HttpGet]
    public async Task<IActionResult> List(string email, string firstName, int page = 1)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
            return AccessDeniedView();
        var role = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        if (role == null) return NotFound();
        page = Math.Clamp(page, 1, 1000000);
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { role.Id },
            email: email, firstName: firstName, pageIndex: page - 1, pageSize: 20);
        var model = new GoldManagementModel { Email = email, FirstName = firstName, Page = page, TotalPages = customers.TotalPages };
        foreach (var customer in customers)
            model.Customers.Add(new GoldManagementModel.CustomerRow {
                Id = customer.Id, Email = customer.Email,
                Name = await _customerService.GetCustomerFullNameAsync(customer),
                Balance = await _goldWalletService.GetBalanceAsync(customer.Id)
            });
        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Admin/Management.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int customerId, int page = 1)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
            return AccessDeniedView();
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null || customer.Deleted) return NotFound();
        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Admin/Details.cshtml", new CustomerGoldWalletModel {
            CustomerId = customer.Id, CustomerName = await _customerService.GetCustomerFullNameAsync(customer),
            HistoryPage = Math.Clamp(page, 1, 1000000)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Adjust(int customerId, string grams, string note)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
            return ErrorJson("شما اجازه دسترسی به این بخش را ندارید.");

        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null || customer.Deleted)
            return ErrorJson("مشتری یافت نشد.");

        // Parsed with the invariant culture on purpose: the admin may type "0.5" or "0,5" or Persian digits.
        if (!TryParseGrams(grams, out var delta))
            return ErrorJson("مقدار طلا معتبر نیست. عدد را مثل 0.5 یا -0.25 وارد کنید.");

        var admin = await _workContext.GetCurrentCustomerAsync();

        var result = await _goldWalletService.AdjustAsync(customer.Id, delta, note, admin.Id);
        if (!result.Success)
            return ErrorJson(result.Error);

        return Json(new { Result = true });
    }

    private static bool TryParseGrams(string input, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var normalized = input.Trim()
            .Replace('٫', '.').Replace('٬', ' ').Replace(',', '.')
            .Replace('−', '-');

        // Persian (۰-۹) and Arabic-Indic (٠-٩) digits -> Latin
        var chars = normalized.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] >= '۰' && chars[i] <= '۹') chars[i] = (char)('0' + (chars[i] - '۰'));
            else if (chars[i] >= '٠' && chars[i] <= '٩') chars[i] = (char)('0' + (chars[i] - '٠'));
        }

        return decimal.TryParse(new string(chars).Replace(" ", string.Empty),
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out value);
    }
}

