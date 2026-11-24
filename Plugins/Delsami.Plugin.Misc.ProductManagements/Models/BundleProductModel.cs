using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace ZarIran.Plugin.Misc.ProductManagements.Models
{
    public record BundleProductModel : BaseNopModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public PictureModel Picture { get; set; }


        public string PriceStr { get; set; }

        public string OldPriceStr { get; set; }

        public decimal DiscountPercent { get; set; }

        public int Count { get; set; }

        public PictureModel ManufacturerPicture { get; set; }

        public string ManufacturerName { get; set; }

        public string ManufacturerSeName { get; set; }
    }
}
