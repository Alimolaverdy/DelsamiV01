using Nop.Web.Models.Common;

namespace ZarIran.Plugin.Misc.OnePageCheckout.Models
{
    public record ZarIranAddressModel : AddressModel
    {
        public int CityId { get; set; }
    }
}
