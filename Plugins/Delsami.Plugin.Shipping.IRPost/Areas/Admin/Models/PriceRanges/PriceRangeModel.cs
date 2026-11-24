using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges
{
    public record PriceRangeModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.MinWieght")]
        public decimal MinWieght { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.ToUpperWeight")]
        public bool ToUpperWeight { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.MaxWeight")]
        public decimal? MaxWeight { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.NeighbouringPrice")]
        public decimal NeighbouringPrice { get; set; }


        [NopResourceDisplayName("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.NonNeighbouringPrice")]
        public decimal NonNeighbouringPrice { get; set; }
    }
}
