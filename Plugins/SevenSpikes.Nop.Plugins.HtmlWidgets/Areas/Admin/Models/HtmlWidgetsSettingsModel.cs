using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;

public class HtmlWidgetsSettingsModel
{
    public bool IsTrialVersion { get; set; }

    public string Theme { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Settings.EnableHtmlWidgets")]
    public bool EnableHtmlWidgets { get; set; }

    public bool EnableHtmlWidgets_OverrideForStore { get; set; }

    public int ActiveStoreScopeConfiguration { get; set; }
}
