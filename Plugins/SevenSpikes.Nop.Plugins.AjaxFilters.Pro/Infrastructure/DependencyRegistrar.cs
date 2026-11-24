using Microsoft.Extensions.DependencyInjection;
using SevenSpikes.Nop.Core.Helpers;
using SevenSpikes.Nop.Framework.DependancyRegistrar;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Settings;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure
{
  public class DependencyRegistrar : BaseDependancyRegistrar7Spikes
  {
    protected override void CreateModelMappings() => this.CreateMvcModelMap<NopAjaxFiltersProSettingsModel, NopAjaxFiltersProSettings>();

    protected override void RegisterPluginServices(IServiceCollection services) => services.AddTransient<IConvertToDictionaryHelper, ConvertToDictionaryHelper>();
  }
}
