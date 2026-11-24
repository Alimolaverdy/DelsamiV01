using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Helpers;
using Nop.Services.Security;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Components;

[ViewComponent(Name = "NopAjaxCart")]
public class NopAjaxCartComponent : ViewComponent
{
    private readonly NopAjaxCartSettings _settings;
    private readonly IPermissionService _permissionService;
    private readonly IWebHelper _webHelper;
    private readonly IUserAgentHelper _userAgentHelper;

    public NopAjaxCartComponent(
        NopAjaxCartSettings settings,
        IPermissionService permissionService,
        IWebHelper webHelper,
        IUserAgentHelper userAgentHelper)
    {
        _settings = settings;
        _permissionService = permissionService;
        _webHelper = webHelper;
        _userAgentHelper = userAgentHelper;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_settings.EnableNopAjaxCart
            || (!_settings.EnableOnProductPage && !_settings.EnableOnCatalogPages)
            || Request.Query.ContainsKey("mobile")
            || (!_settings.EnableOnMobileDevices && _userAgentHelper.IsMobileDevice()))
        {
            return Content(string.Empty);
        }

        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices)
            || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
        {
            return Content(string.Empty);
        }

        var themeRollerCookie = Request.Cookies["cookie_theme_roller_iframe"];
        var referrer = _webHelper.GetUrlReferrer();
        if (!string.IsNullOrWhiteSpace(themeRollerCookie)
            && referrer != null
            && Request.GetDisplayUrl() != null
            && string.Equals(Request.Host.Host, new Uri(referrer).Host, StringComparison.OrdinalIgnoreCase))
        {
            return Content(string.Empty);
        }

        return View("NopAjaxCart");
    }
}
