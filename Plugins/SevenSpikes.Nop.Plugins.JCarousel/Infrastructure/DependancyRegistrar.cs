using Microsoft.Extensions.DependencyInjection;
using SevenSpikes.Nop.Framework.DependancyRegistrar;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Services;

namespace SevenSpikes.Nop.Plugins.JCarousel.Infrastructure;

public class DependancyRegistrar : BaseDependancyRegistrar7Spikes
{
    protected override void CreateModelMappings()
    {
        CreateMvcModelMap<JCarouselSettingsModel, Carousel>();
        CreateMvcModelMap<JCarouselGeneralSettingsModel, JCarouselGeneralSettings>();
        CreateMvcModelMap<JCarouselEntitySettingsModel, JCarouselEntitySettings>();
    }

    protected override void RegisterPluginServices(IServiceCollection services)
    {
        services.AddTransient<IJCarouselService, JCarouselService>();
        services.AddTransient<IItemsMappingService, ItemsMappingService>();
    }
}
