using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;

public record HtmlWidgetSearchModel : BaseSearchModel
{
    public bool IsTrialVersion { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.SearchName")]
    public string SearchWidgetName { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.SearchContent")]
    public string SearchWidgetContent { get; set; }

    public HtmlWidgetSearchModel()
    {
    }
}
