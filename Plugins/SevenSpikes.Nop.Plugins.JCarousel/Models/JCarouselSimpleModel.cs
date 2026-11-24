using System.Collections.Generic;

namespace SevenSpikes.Nop.Plugins.JCarousel.Models;

public class JCarouselSimpleModel
{
    public IList<JCarouselItemModel> Items { get; set; }

    public string DataSourceType { get; set; }

    public JCarouselSimpleModel()
    {
        Items = new List<JCarouselItemModel>();
    }
}
