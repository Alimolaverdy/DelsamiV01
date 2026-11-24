using Nop.Core;

namespace ZarIran.Plugin.Shipping.Tipax.Domain
{
    public class ZarIranTipaxCityMapping : BaseEntity
    {
        public string Name { get; set; }

        public int StateProvinceId { get; set; }

        public int ZarIranCityId { get; set; }

        public int TipaxCityId { get; set; }
    }
}
