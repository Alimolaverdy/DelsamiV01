using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;

public class JCarouselSettingsLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("SevenSpikes.JCarousel.Admin.Settings.Title")]
    public string Title { get; set; }
}
