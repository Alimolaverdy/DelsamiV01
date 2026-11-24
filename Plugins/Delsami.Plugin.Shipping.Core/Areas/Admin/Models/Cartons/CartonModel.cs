using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Shipping.Core.Areas.Admin.Models.Cartons
{
    public record CartonModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShippingCore.Admin.CartonModel.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShippingCore.Admin.CartonModel.Width")]
        public decimal Width { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShippingCore.Admin.CartonModel.Height")]
        public decimal Height { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShippingCore.Admin.CartonModel.Length")]
        public decimal Length { get; set; } 
        
        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShippingCore.Admin.CartonModel.PostBoxNumber")]
        public int PostBoxNumber { get; set; }
    }
}
