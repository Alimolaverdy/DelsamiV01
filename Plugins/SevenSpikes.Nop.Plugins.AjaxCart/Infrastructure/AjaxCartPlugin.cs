using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Framework.Plugin;
using SevenSpikes.Nop.Plugins.AjaxCart.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Infrastructure
{
    public class AjaxCartPlugin : BaseAdminWidgetPlugin7Spikes
    {
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;
        private static readonly List<MenuItem7Spikes> MenuItems = new List<MenuItem7Spikes>()
    {
      new MenuItem7Spikes()
      {
        SubMenuName = "SevenSpikes.NopAjaxCart.Admin.Submenus.Settings",
        SubMenuRelativePath = "NopAjaxCartAdmin/Settings"
      }
    };

        private static bool IsTrialVersion => false;

        public AjaxCartPlugin(WidgetSettings widgetSettings, ISettingService settingService)
          : base(AjaxCartPlugin.MenuItems, "SevenSpikes.Plugins.AjaxCart.Admin.Menu.MenuName", "SevenSpikes.Nop.Plugins.AjaxCart", AjaxCartPlugin.IsTrialVersion, "http://www.nop-templates.com/ajax-cart-plugin-for-nopcommerce")
        {
            this._widgetSettings = widgetSettings;
            this._settingService = settingService;
        }

        public override string GetConfigurationPageUrl() => this.StoreLocation + "Admin/NopAjaxCartAdmin/Settings";

        public override Type GetWidgetViewComponent(string widgetZone) => widgetZone == "content_before" ? typeof(NopAjaxCartComponent) : typeof(NopAjaxCartAddProductToCartAjaxButtonScriptComponent);

        protected override async Task InstallAdditionalSettingsAsync()
        {
            if (this._widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.AjaxCart"))
                return;
            this._widgetSettings.ActiveWidgetSystemNames.Add("SevenSpikes.Nop.Plugins.AjaxCart");
            await this._settingService.SaveSettingAsync<WidgetSettings>(this._widgetSettings, 0);
        }
    }
}
