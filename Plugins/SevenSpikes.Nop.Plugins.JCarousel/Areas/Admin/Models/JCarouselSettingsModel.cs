using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using SevenSpikes.Nop.Framework.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Models;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;

public record JCarouselSettingsModel : BaseNopEntityModel, ILocalizedModel<JCarouselSettingsLocalizedModel>, ILocalizedModel
{
    public JCarouselProductMappingModel ProductMapping { get; set; }

    public bool IsTrialVersion { get; set; }

    public JCarouselToWidgetModel AddCarouselToWidget { get; set; }

    public string CarouselHtmlElementId { get; set; }

    public IList<JCarouselToWidgetModel> Widgets { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.WidgetZoneNames")]
    public string WidgetZoneNames => string.Join(" | ", Widgets.Select((JCarouselToWidgetModel x) => x.WidgetZone));

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.DataSourceType")]
    public string DataSourceType { get; set; }

    public IList<SelectListItem> AvailableAnimationSpeeds => GetAvailableAnimationSpeeds();

    public IList<SelectListItem> AvailableEasings => GetAvailableEasings();

    public IList<SelectListItem> AvailableDataSourceTypes { get; set; }

    public IList<JCarouselSettingsLocalizedModel> Locales { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Fields.LimitedToStores")]
    public bool LimitedToStores { get; set; }

    public StoreMappingModel StoreMappingModel { get; set; }

    public JCarouselEntitySettingsModel JCarouselEntitySettings { get; set; }

    public JCarouselSettingsModel()
    {
        Widgets = new List<JCarouselToWidgetModel>();
        Locales = new List<JCarouselSettingsLocalizedModel>();
        AddCarouselToWidget = new JCarouselToWidgetModel();
        ProductMapping = new JCarouselProductMappingModel();
        StoreMappingModel = new StoreMappingModel();
    }

    private static IList<SelectListItem> GetAvailableAnimationSpeeds()
    {
        return new List<string> { "none", "slow", "fast" }.Select((string animationSpeed) => new SelectListItem
        {
            Text = animationSpeed,
            Value = animationSpeed
        }).ToList();
    }

    private static IList<SelectListItem> GetAvailableEasings()
    {
        return new List<string> { "none", "linear", "swing" }.Select((string easing) => new SelectListItem
        {
            Text = easing,
            Value = easing
        }).ToList();
    }
}
