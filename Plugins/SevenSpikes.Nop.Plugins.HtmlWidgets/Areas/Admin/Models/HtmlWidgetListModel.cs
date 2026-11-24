using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nop.Web.Framework.Models;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;

public record HtmlWidgetListModel : BasePagedListModel<HtmlWidgetModel>
{
    public bool IsTrialVersion { get; set; }

    public HtmlWidgetListModel()
    {
    }
}
