using Nop.Web.Models.Common;

namespace ZarIran.Plugin.Widgets.AddressLocation.Models.Addresses
{
    public record ZarIranAddressModel : AddressModel
    {
        public int CityId { get; set; }
    }
}
