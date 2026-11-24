using Nop.Web.Models.Catalog;
using ZarIran.Plugin.Misc.ProductManagements.Models;
using static Nop.Web.Models.Catalog.CategoryModel;

namespace SevenSpikes.Nop.Plugins.JCarousel.Models;

public class JCarouselItemModel
{
    public ZarIranProductOverviewModel ProductOverviewModel { get; set; }

    public ManufacturerModel ManufacturerModel { get; set; }

    public VendorModel VendorModel { get; set; }

    public SubCategoryModel CategoryModel { get; set; }

    public JCarouselItemModel()
    {
        ProductOverviewModel = new ZarIranProductOverviewModel();
        ManufacturerModel = new ManufacturerModel();
        VendorModel = new VendorModel();
        CategoryModel = new SubCategoryModel();
    }
}
