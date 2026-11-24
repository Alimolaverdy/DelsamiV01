using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.Core.Bases
{
    public interface ICustomerNavigationItems
    {
        Task<List<CustomerNavigationItemModel>> PrepareCustomerNavigationItemsAsync();

    }
}
