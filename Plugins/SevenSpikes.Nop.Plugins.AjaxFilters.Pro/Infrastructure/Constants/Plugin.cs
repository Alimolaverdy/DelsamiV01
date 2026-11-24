using SevenSpikes.Nop.Framework.Plugin;
using System.Collections.Generic;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Constants
{
  public class Plugin
  {
    public const string Name = "Nop Ajax Filters Pro";
    public const string ResourceName = "SevenSpikes.Plugins.AjaxFiltersPro.Admin.Menu.MenuName";
    public const string SystemName = "SevenSpikes.Nop.Plugins.AjaxFilters.Pro";
    public const string FolderName = "SevenSpikes.Nop.Plugins.AjaxFilters.Pro";
    public const string UrlInStore = "http://www.nop-templates.com/ajax-filters-pro-plugin-for-nopcommerce";

    public static List<MenuItem7Spikes> MenuItems => new List<MenuItem7Spikes>()
    {
      new MenuItem7Spikes()
      {
        SubMenuName = "SevenSpikes.NopAjaxFiltersPro.Admin.Submenus.Settings",
        SubMenuRelativePath = "AjaxFiltersProAdmin/Settings"
      }
    };

    public static bool IsTrialVersion => false;
  }
}
