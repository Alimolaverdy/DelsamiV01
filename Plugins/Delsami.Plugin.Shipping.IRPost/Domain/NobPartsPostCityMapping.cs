using Nop.Core;

namespace ZarIran.Plugin.Shipping.IRPost.Domain
{
    public class ZarIranPostCityMapping : BaseEntity
    {
        public string Name { get; set; }

        public int StateProvinceId { get; set; }

        public int ZarIranCityId { get; set; }

        public int PostCityId { get; set; }
    }
}
