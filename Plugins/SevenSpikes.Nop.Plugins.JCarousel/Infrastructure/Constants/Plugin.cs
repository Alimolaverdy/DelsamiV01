using SevenSpikes.Nop.Framework.Domain.Enums;

namespace SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;

public class Plugin
{
    public const string Name = "Nop JCarousel";

    public const string ResourceName = "SevenSpikes.Plugins.JCarousel.Admin.Menu.MenuName";

    public const string FolderName = "SevenSpikes.Nop.Plugins.JCarousel";

    public const string SystemName = "SevenSpikes.Nop.Plugins.JCarousel";

    public static EntityType EntityType = EntityType.Carousel;

    public const string JCarouselContextName = "7spikes_jcarousel_object_context";

    public const string JCarouselSkinLocation = "~/Plugins/SevenSpikes.Nop.Plugins.JCarousel/Themes/{0}/Skins";

    public const string ControllersNamespace = "SevenSpikes.Nop.Plugins.JCarousel.Controllers";

    public const string JCarouselTableName = "SS_JC_JCarousel";

    public const string UrlInStore = "http://www.nop-templates.com/jcarousel-plugin-for-nopcommerce";

    public const string DefaultResponsiveBreakpointsKey = "JCarouselGeneralSettings.DefaultResponsiveBreakpointsForSlickSlider";

    public const string ThemeResponsiveBreakpointsKey = "JCarouselGeneralSettings.ResponsiveBreakpointsForSlickSlider-{0}";

    public const string CategorySliderResponsiveBreakpointsKey = "JCarouselGeneralSettings.ResponsiveBreakpointsCategorySlider-{0}";

    public const string ManufacturerSliderResponsiveBreakpointsKey = "JCarouselGeneralSettings.ResponsiveBreakpointsManufacturerSlider-{0}";

    public const string VendorSliderResponsiveBreakpointsKey = "JCarouselGeneralSettings.ResponsiveBreakpointsVendorSlider-{0}";

    public const string CategoryDataSourceTypeClass = "category-grid";

    public const string ManufacturerDataSourceTypeClass = "manufacturer-grid";

    public const string VendorDataSourceTypeClass = "vendor-grid";

    public const string ProductDataSourceTypeClass = "product-grid";
}
