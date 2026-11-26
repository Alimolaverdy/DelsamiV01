using Nop.Services.Common.Pdf;

namespace Delsami.Plugin.Misc.CustomPdfGenerator.Model
{
    public class CustomProductItem : ProductItem
    {
        public int VendorId { get; set; }

        public string UnitPriceExcludeTax { get; set; }

        public string UnitPriceIncludeTax { get; set; }

        public string PriceIncludeTax { get; set; }

        public string PriceExcludeTax { get; set; }

        public string Tax { get; set; }

        public string DiscountAmountExcludeTax { get; set; }

        //public string TotalPriceIncludeDiscountExcludeTax { get; set; }

        //public string Tax { get; set; }

    }
}
