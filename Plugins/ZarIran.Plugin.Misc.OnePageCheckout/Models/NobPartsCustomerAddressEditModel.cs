using Nop.Web.Framework.Models;
using Nop.Web.Models.Customer;

namespace ZarIran.Plugin.Misc.OnePageCheckout.Models
{
    public record ZarIranCustomerAddressEditModel : BaseNopModel
    {
        public ZarIranAddressModel Address { get; set; }
    }
}
