using SevenSpikes.Nop.Framework.AutoMapper;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Extensions;

public static class MappingExtensions
{
    public static JCarouselEntitySettingsModel ToModel(this JCarouselEntitySettings jCarouselEntitySettings)
    {
        return jCarouselEntitySettings.MapTo<JCarouselEntitySettings, JCarouselEntitySettingsModel>();
    }

    public static JCarouselSettingsModel ToModel(this Carousel entity)
    {
        return entity.MapTo<Carousel, JCarouselSettingsModel>();
    }

    public static Carousel ToEntity(this JCarouselSettingsModel model)
    {
        return model.MapTo<JCarouselSettingsModel, Carousel>();
    }

    public static Carousel ToEntity(this JCarouselSettingsModel model, Carousel destination)
    {
        return model.MapTo(destination);
    }

    public static JCarouselGeneralSettings ToEntity(this JCarouselGeneralSettingsModel model)
    {
        return model.MapTo<JCarouselGeneralSettingsModel, JCarouselGeneralSettings>();
    }

    public static JCarouselGeneralSettingsModel ToModel(this JCarouselGeneralSettings entity)
    {
        return entity.MapTo<JCarouselGeneralSettings, JCarouselGeneralSettingsModel>();
    }
}
