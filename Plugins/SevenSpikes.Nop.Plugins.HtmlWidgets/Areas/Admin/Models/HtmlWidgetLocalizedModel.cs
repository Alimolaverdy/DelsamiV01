using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;

public class HtmlWidgetLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.HtmlContent")]
    public string HtmlContent { get; set; }
}
