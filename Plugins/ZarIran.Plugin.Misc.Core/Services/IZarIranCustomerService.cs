using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.Core.Services
{
    public interface IZarIranCustomerService
    {
        string GetNationalCode(string attributesXml);
        Task<bool> IsCheckoutCompletedAsync(Customer customer);
    }
}
