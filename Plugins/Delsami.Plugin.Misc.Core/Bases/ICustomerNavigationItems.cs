using Nop.Web.Models.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Bases
{
    public interface ICustomerNavigationItems
    {
        Task<List<CustomerNavigationItemModel>> PrepareCustomerNavigationItemsAsync();

    }
}
