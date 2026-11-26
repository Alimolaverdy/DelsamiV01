using Nop.Core;

namespace Delsami.Plugin.Shipping.IRPost.Domain
{
    public class DelsamiPostCityMapping : BaseEntity
    {
        public string Name { get; set; }

        public int StateProvinceId { get; set; }

        public int DelsamiCityId { get; set; }

        public int PostCityId { get; set; }
    }
}
