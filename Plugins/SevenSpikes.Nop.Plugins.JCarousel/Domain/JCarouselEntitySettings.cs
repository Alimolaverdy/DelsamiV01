using Nop.Core.Configuration;
using SevenSpikes.Nop.EntitySettings.MarkerInterfaces;

namespace SevenSpikes.Nop.Plugins.JCarousel.Domain;

public class JCarouselEntitySettings : ISettings, IEntitySettings
{
    public int NumberOfItems { get; set; }

    public int NumberOfVisibleItems { get; set; }

    public string Skin { get; set; }

    public int StartIndex { get; set; }

    public int ScrollItems { get; set; }

    public int Autoscroll { get; set; }

    public bool AutoscrollHoverPause { get; set; }

    public bool LoopItems { get; set; }

    public string WrapItems { get; set; }

    public string AnimationSpeed { get; set; }

    public bool NavigationDots { get; set; }

    public bool NavigationArrows { get; set; }

    public int ProductImageSize { get; set; }

    public bool ShowTitle { get; set; }
}
