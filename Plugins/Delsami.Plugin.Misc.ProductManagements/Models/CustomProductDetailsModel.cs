using Nop.Web.Models.Catalog;

namespace ZarIran.Plugin.Misc.ProductManagements.Models
{
    public record CustomProductDetailsModel : ProductDetailsModel
    {
        public string Distance { get; set; }

        public int DistanceAsInt { get; set; }
    }
}
