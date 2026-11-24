using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.JCarousel.Models;

public record JCarouselToWidgetModel : BaseNopEntityModel
{
    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.WidgetZones.WidgetZone")]
    public string WidgetZone { get; set; }

    public SelectList SupportedWidgetZones { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.WidgetZones.DisplayOrder")]
    public int DisplayOrder { get; set; }

}
