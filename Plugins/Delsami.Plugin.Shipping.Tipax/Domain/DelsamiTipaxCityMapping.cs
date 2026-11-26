using Nop.Core;

namespace Delsami.Plugin.Shipping.Tipax.Domain
{
    public class DelsamiTipaxCityMapping : BaseEntity
    {
        public string Name { get; set; }

        public int StateProvinceId { get; set; }

        public int DelsamiCityId { get; set; }

        public int TipaxCityId { get; set; }
    }
}
