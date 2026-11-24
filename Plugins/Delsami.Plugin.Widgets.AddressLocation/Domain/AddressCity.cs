using Nop.Core;

namespace ZarIran.Plugin.Widgets.AddressLocation.Domain
{
    public class AddressCity : BaseEntity
    {
        public int AddressId { get; set; }

        public int CityId { get; set; }
    }
}
