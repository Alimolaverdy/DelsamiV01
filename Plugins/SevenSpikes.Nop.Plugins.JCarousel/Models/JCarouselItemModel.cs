using Nop.Web.Models.Catalog;
using static Nop.Web.Models.Catalog.CategoryModel;
using Delsami.Plugin.Misc.ProductManagement.Models;

namespace SevenSpikes.Nop.Plugins.JCarousel.Models;

public class JCarouselItemModel
{
    public DelsamiProductOverviewModel ProductOverviewModel { get; set; }

    public ManufacturerModel ManufacturerModel { get; set; }

    public VendorModel VendorModel { get; set; }

    public SubCategoryModel CategoryModel { get; set; }

    public JCarouselItemModel()
    {
        ProductOverviewModel = new DelsamiProductOverviewModel();
        ManufacturerModel = new ManufacturerModel();
        VendorModel = new VendorModel();
        CategoryModel = new SubCategoryModel();
    }
}
