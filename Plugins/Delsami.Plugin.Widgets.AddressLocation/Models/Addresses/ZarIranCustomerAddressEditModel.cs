using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Widgets.AddressLocation.Models.Addresses
{
    public record ZarIranCustomerAddressEditModel : BaseNopModel
    {
        public ZarIranCustomerAddressEditModel()
        {
            Address = new();
        }

        public ZarIranAddressModel Address { get; set; }
    }
}
