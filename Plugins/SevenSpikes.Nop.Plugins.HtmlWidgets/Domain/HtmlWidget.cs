using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;

public class HtmlWidget : BaseEntity, ILocalizedEntity, IStoreMappingSupported
{
    public string Name { get; set; }

    public bool Visible { get; set; }

    public bool UseJavaScript { get; set; }

    public string JavaScriptContent { get; set; }

    public string HtmlContent { get; set; }

    public bool LimitedToStores { get; set; }
}
