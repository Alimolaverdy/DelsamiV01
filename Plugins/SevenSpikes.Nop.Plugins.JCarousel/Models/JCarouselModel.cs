using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Framework.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;

namespace SevenSpikes.Nop.Plugins.JCarousel.Models;

public record JCarouselModel : Base7SpikesProductsModel
{
    public JCarouselSettingsModel Settings { get; set; }

    public JCarouselGeneralSettingsModel GeneralSettings { get; set; }

    public IList<JCarouselItemModel> Items { get; set; }

    public string DataSourceTypeClass { get; set; }

    public string Theme { get; set; }

    public string ResponsiveBreakpoints { get; set; }

    public string NavigationArrowsSelector { get; set; }

    public JCarouselModel()
    {
        Settings = new JCarouselSettingsModel();
        GeneralSettings = new JCarouselGeneralSettingsModel();
        Items = new List<JCarouselItemModel>();
    }

    public override IList<ProductOverviewModel> GetProductOverviewModels(string consumerName)
    {
        List<ProductOverviewModel> list = new List<ProductOverviewModel>();
        if (!consumerName.Equals("ProductBoxPicturesCarousel"))
        {
            foreach (JCarouselItemModel item in Items)
            {
                list.Add(item.ProductOverviewModel);
            }
        }
        return list;
    }

}
