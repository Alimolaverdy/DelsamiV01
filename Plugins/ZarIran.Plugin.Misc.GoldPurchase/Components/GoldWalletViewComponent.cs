using Microsoft.AspNetCore.Mvc;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Components;
using System;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.GoldPurchase.Enums;
using ZarIran.Plugin.Misc.GoldPurchase.Models;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Components;

/// <summary>
/// Renders the "Gold wallet" card on the admin customer edit page
/// (widget zone admin_customer_details_block, next to the Reward points card).
/// </summary>
public class GoldWalletViewComponent : NopViewComponent
{
    private readonly IGoldWalletService _goldWalletService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IPermissionService _permissionService;

    public GoldWalletViewComponent(IGoldWalletService goldWalletService, IDateTimeHelper dateTimeHelper, IPermissionService permissionService)
    {
        _goldWalletService = goldWalletService;
        _dateTimeHelper = dateTimeHelper;
        _permissionService = permissionService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData, int historyPage = 1)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
            return Content(string.Empty);
        historyPage = Math.Clamp(historyPage, 1, 1000000);
        // additionalData is the admin CustomerModel; Id is 0 on the "add customer" page -> show nothing there.
        if (additionalData is not CustomerModel customerModel || customerModel.Id <= 0)
            return Content(string.Empty);

        var model = new CustomerGoldWalletModel
        {
            CustomerId = customerModel.Id,
            BalanceGrams = await _goldWalletService.GetBalanceAsync(customerModel.Id),
            HistoryPage = historyPage,
            HistoryPages = (int)Math.Ceiling(await _goldWalletService.GetHistoryCountAsync(customerModel.Id) / 50m)
        };

        foreach (var t in await _goldWalletService.GetHistoryAsync(customerModel.Id, 50, historyPage - 1))
        {
            var local = await _dateTimeHelper.ConvertToUserTimeAsync(t.CreatedOnUtc, DateTimeKind.Utc);

            model.Rows.Add(new CustomerGoldWalletModel.Row
            {
                CreatedOn = (local),
                Type = t.TransactionType switch
                {
                    TransactionType.Purchase => "خرید",
                    TransactionType.Sell => "فروش",
                    TransactionType.AdminAdjustment => "اصلاح توسط مدیر",
                    _ => t.TransactionType.ToString()
                },
                Grams = t.GoldWeightGrams,
                BalanceAfterGrams = t.GoldBalanceAfterGrams,
                PricePerGram = t.PricePerGram,
                Amount = t.Amount,
                OrderId = t.OrderId,
                AdminCustomerId = t.AdminCustomerId,
                Note = t.Note
            });
        }

        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Admin/CustomerGoldWallet.cshtml", model);
    }
}
