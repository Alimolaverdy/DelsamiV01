using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Domain
{
    public class ShippingMethodCityRestriction : BaseEntity
    {
        [NopResourceDisplayName("Delsami.Plugin.Shipping.ManageShippingMethod.ShippingMethodStateProvinceRestrictionModel.ShippingMethodId")]

        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.ManageShippingMethod.ShippingMethodStateProvinceRestrictionModel.StateProvinceId")]
        public int CityId { get; set; }
    }
}
