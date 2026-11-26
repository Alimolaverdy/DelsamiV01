using Nop.Web.Models.Catalog;

namespace Delsami.Plugin.Misc.ProductManagement.Models
{
    public record CustomProductDetailsModel : ProductDetailsModel
    {
        public string Distance { get; set; }

        public int DistanceAsInt { get; set; }
    }
}
