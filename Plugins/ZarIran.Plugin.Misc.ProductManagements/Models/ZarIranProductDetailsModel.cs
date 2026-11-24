using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ProductManagements.Models
{
    public record ZarIranProductDetailsModel : ProductDetailsModel
    {
        public string AdminComment { get; set; }

        public List<BundleProductModel> BundleProducts { get; set; } = new();

        public string BDPCategoriesName { get; set; }
    }
}
