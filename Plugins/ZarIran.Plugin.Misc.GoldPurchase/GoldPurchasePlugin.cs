using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Infrastructure;
using ZarIran.Plugin.Misc.GoldPurchase.Components;

namespace ZarIran.Plugin.Misc.GoldPurchase;

// IWidgetPlugin is what lets this plugin add the "Gold wallet" card to the admin customer page,
// so no second plugin is needed.
public class GoldPurchasePlugin : BasePlugin, IMiscPlugin, IWidgetPlugin, IAdminMenuPlugin
{
    private readonly IPermissionService _permissionService;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly IProductService _productService;
    private readonly GoldPurchaseSettings _settings;
    private readonly IPaymentPluginManager _paymentPluginManager;

    public GoldPurchasePlugin(
        ILocalizationService localizationService,
        ISettingService settingService,
        IWebHelper webHelper,
        IProductService productService,
        GoldPurchaseSettings settings,
        IPaymentPluginManager paymentPluginManager,
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
        _localizationService = localizationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _productService = productService;
        _settings = settings;
        _paymentPluginManager = paymentPluginManager;
    }

    public async Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        var node = new SiteMapNode { SystemName = "ZarIran.GoldPurchase", Title = "خرید و فروش طلا", Visible = true, IconClass = "fas fa-coins" };
        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
            node.ChildNodes.Add(new SiteMapNode {
                SystemName = "ZarIran.GoldPurchase.Management", Title = "مدیریت", Visible = true,
                ControllerName = "GoldWalletAdmin", ActionName = "List", IconClass = "far fa-user",
                RouteValues = new RouteValueDictionary { ["area"] = "Admin" }
            });
        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            node.ChildNodes.Add(new SiteMapNode {
                SystemName = "ZarIran.GoldPurchase.Withdrawals", Title = "درخواست برداشت وجه", Visible = true,
                ControllerName = "GoldWithdrawalAdmin", ActionName = "List", IconClass = "fas fa-money-bill-wave",
                RouteValues = new RouteValueDictionary { ["area"] = "Admin" }
            });
        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            node.ChildNodes.Add(new SiteMapNode {
                SystemName = "ZarIran.GoldPurchase.Settings", Title = "تنظیمات", Visible = true,
                ControllerName = "GoldPurchase", ActionName = "Configure", IconClass = "fas fa-cog",
                RouteValues = new RouteValueDictionary { ["area"] = "Admin" }
            });
        if (node.ChildNodes.Any()) rootNode.ChildNodes.Add(node);
    }

    public override string GetConfigurationPageUrl()
        => $"{_webHelper.GetStoreLocation()}Admin/GoldPurchase/Configure";

    #region IWidgetPlugin

    // The plugin manages widget activation during installation and upgrades.
    public bool HideInWidgetList => true;

    public Task<IList<string>> GetWidgetZonesAsync()
        => Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.CustomerDetailsBlock, PublicWidgetZones.ContentBefore });

    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == AdminWidgetZones.CustomerDetailsBlock)
            return typeof(GoldWalletViewComponent);
        if (widgetZone == PublicWidgetZones.ContentBefore)
            return typeof(GoldWithdrawalLinkViewComponent);
        return null;
    }

    #endregion

    private async Task SetWidgetActiveAsync(bool active)
    {
        // A widget is only rendered when its system name is in WidgetSettings.ActiveWidgetSystemNames.
        var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>();
        var systemName = PluginDescriptor.SystemName;
        var changed = false;

        if (active && !widgetSettings.ActiveWidgetSystemNames.Contains(systemName))
        {
            widgetSettings.ActiveWidgetSystemNames.Add(systemName);
            changed = true;
        }
        else if (!active && widgetSettings.ActiveWidgetSystemNames.Remove(systemName))
        {
            changed = true;
        }

        if (changed)
            await _settingService.SaveSettingAsync(widgetSettings);
    }

    private Task AddResourcesAsync()
        => _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.GoldProductId"] = "شناسه محصول خرید طلا",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.DefaultAmount"] = "مبلغ پیش‌فرض",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.MinimumAmount"] = "حداقل مبلغ",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.MaximumAmount"] = "حداکثر مبلغ",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.PaymentSystemName"] = "روش پرداخت",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.IsActive"] = "فعال بودن خرید و فروش طلا",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.BuyCommission"] = "کارمزد خرید (درصد)",
            ["ZarIran.Plugin.Misc.GoldPurchase.Fields.SellCommission"] = "کارمزد فروش (درصد)"
        });

    public override async Task UpdateAsync(string currentVersion, string targetVersion)
    {
        // Runs once when an already-installed 1.00 is upgraded (after UpdateMigration_1_10).
        await AddResourcesAsync();
        await SetWidgetActiveAsync(true);
    }

    public override async Task InstallAsync()
    {
        var product = new Product
        {
            AllowedQuantities = "1",
            CustomerEntersPrice = true,
            IsTaxExempt = true,
            IsShipEnabled = false,
            Name = "خرید طلا",
            ShortDescription = "محصول سیستمی خرید طلا",
            FullDescription = "این محصول توسط پلاگین خرید طلا استفاده می‌شود.",
            Published = true,
            OrderMaximumQuantity = 1,
            ProductTypeId = 5,
            MinimumCustomerEnteredPrice = 100000,
            MaximumCustomerEnteredPrice = 1000000000
        };

        await _productService.InsertProductAsync(product);

        _settings.GoldProductId = product.Id;
        _settings.DefaultAmount = 1000000;
        _settings.MinimumAmount = 100000;
        _settings.MaximumAmount = 1000000000;

        var payments = (await _paymentPluginManager.LoadActivePluginsAsync())
            .Where(x => x.PaymentMethodType == PaymentMethodType.Redirection)
            .ToList();

        _settings.PaymentSystemName = payments.FirstOrDefault()?.PluginDescriptor.SystemName;
        await _settingService.SaveSettingAsync(_settings);

        await AddResourcesAsync();
        await SetWidgetActiveAsync(true);

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await SetWidgetActiveAsync(false);
        await _settingService.DeleteSettingAsync<GoldPurchaseSettings>();
        await _localizationService.DeleteLocaleResourcesAsync(
            "ZarIran.Plugin.Misc.GoldPurchase");
        await base.UninstallAsync();
    }
}
