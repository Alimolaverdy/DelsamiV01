using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ProductManagement.Models
{
    public record DelsamiProductDetailsModel : ProductDetailsModel
    {
        public string AdminComment { get; set; }

        public List<BundleProductModel> BundleProducts { get; set; } = new();

        public string BDPCategoriesName { get; set; }
    }
}
