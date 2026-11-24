using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nop.Web.Framework.Models;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Models;

public record HtmlWidgetsPublicModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string HtmlContent { get; set; }

    public bool UseJavaScript { get; set; }

    public string JavaScriptContent { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public HtmlWidgetsPublicModel()
    {
    }
}
