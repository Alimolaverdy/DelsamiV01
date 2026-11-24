using Microsoft.Extensions.DependencyInjection;
using SevenSpikes.Nop.Framework.DependancyRegistrar;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Models;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Services;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure;

public class DependancyRegister : BaseDependancyRegistrar7Spikes
{
    protected override void RegisterPluginServices(IServiceCollection services)
    {
        services.AddTransient<IHtmlWidgetService, HtmlWidgetService>();
    }

    protected override void CreateModelMappings()
    {
        CreateMvcModelMap<HtmlWidgetsSettingsModel, HtmlWidgetsSettings>();
        CreateMvcModelMap<HtmlWidgetModel, HtmlWidget>();
        CreateMvcModelMap<HtmlWidgetsPublicModel, HtmlWidget>();
    }
}
