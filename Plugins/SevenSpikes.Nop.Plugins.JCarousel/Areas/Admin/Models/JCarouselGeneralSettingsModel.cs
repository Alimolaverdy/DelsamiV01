using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;

public class JCarouselGeneralSettingsModel
{
    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.GeneralSettings.Enable")]
    public bool Enable { get; set; }

    public bool Enable_OverrideForStore { get; set; }

    public bool IsTrialVersion { get; set; }

    public int ActiveStoreScopeConfiguration { get; set; }
}
