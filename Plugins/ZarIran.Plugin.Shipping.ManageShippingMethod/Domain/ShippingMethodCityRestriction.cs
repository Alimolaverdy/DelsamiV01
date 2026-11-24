using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Domain
{
    public class ShippingMethodCityRestriction : BaseEntity
    {
        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ManageShippingMethod.ShippingMethodStateProvinceRestrictionModel.ShippingMethodId")]

        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ManageShippingMethod.ShippingMethodStateProvinceRestrictionModel.StateProvinceId")]
        public int CityId { get; set; }
    }
}
