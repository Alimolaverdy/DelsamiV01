using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using SevenSpikes.Nop.Framework.Areas.Admin.Models;
using System.Collections.Generic;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;

public record HtmlWidgetModel : BaseNopEntityModel, ILocalizedModel<HtmlWidgetLocalizedModel>, ILocalizedModel, ICopyableEntityModel
{
    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.Visible")]
    public bool Visible { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.UseJavaScript")]
    public bool UseJavaScript { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.HtmlContent")]
    public string HtmlContent { get; set; }

    [NopResourceDisplayName("SevenSpikes.Plugins.HtmlWidgets.Admin.Entities.HtmlWidget.JavaScriptContent")]
    public string JavaScriptContent { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Fields.LimitedToStores")]
    public bool LimitedToStores { get; set; }

    public bool IsTrialVersion { get; set; }

    public IList<HtmlWidgetLocalizedModel> Locales { get; set; }

    public CopyModel CopyModel { get; set; }

    public StoreMappingModel StoreMappingModel { get; set; }

    public HtmlWidgetModel()
    {
        Locales = new List<HtmlWidgetLocalizedModel>();
        CopyModel = new CopyModel();
        StoreMappingModel = new StoreMappingModel();
        Visible = true;
    }

}
