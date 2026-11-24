using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Services
{
    public interface ICustomerProfileValidationService
    {
        Task<bool> IsCheckoutCompletedAsync(Customer customer);

    }
}
