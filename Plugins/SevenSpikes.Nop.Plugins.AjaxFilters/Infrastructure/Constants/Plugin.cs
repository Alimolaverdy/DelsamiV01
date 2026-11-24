using System.Collections.Generic;
using SevenSpikes.Nop.Framework.Plugin;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Constants;

public class Plugin
{
    public const string SystemName = "SevenSpikes.Nop.Plugins.AjaxFilters";

    public const string FolderName = "SevenSpikes.Nop.Plugins.AjaxFilters";

    public const string Name = "Nop Ajax Filters";

    public const string ResourceName = "SevenSpikes.Plugins.AjaxFilters.Admin.Menu.MenuName";

    public const string UrlInStore = "http://www.nop-templates.com/ajax-filters-plugin-for-nopcommerce";

    public static List<MenuItem7Spikes> MenuItems
    {
        get
        {
            List<MenuItem7Spikes> list = new List<MenuItem7Spikes>();
            MenuItem7Spikes item = default(MenuItem7Spikes);
            item.SubMenuName = "SevenSpikes.NopAjaxFilters.Admin.Submenus.Settings";
            item.SubMenuRelativePath = "NopAjaxFiltersAdmin/Settings";
            list.Add(item);
            return list;
        }
    }

    public static bool IsTrialVersion => false;
}
