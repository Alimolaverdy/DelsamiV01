using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;

public record JCarouselEntitySettingsModel : BaseNopModel
{
    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.NumberOfItems")]
    public int NumberOfItems { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.NumberOfVisibleItems")]
    public int NumberOfVisibleItems { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.Skin")]
    public string Skin { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.StartIndex")]
    public int StartIndex { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.ScrollItems")]
    public int ScrollItems { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.AutoscrollInSeconds")]
    public int Autoscroll { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.AutoscrollHoverPause")]
    public bool AutoscrollHoverPause { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.LoopItems")]
    public bool LoopItems { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.AnimationSpeed")]
    public string AnimationSpeed { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.NavigationDots")]
    public bool NavigationDots { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.NavigationArrows")]
    public bool NavigationArrows { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.ShowTitle")]
    public bool ShowTitle { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.ProductImageSize")]
    public int ProductImageSize { get; set; }

}
