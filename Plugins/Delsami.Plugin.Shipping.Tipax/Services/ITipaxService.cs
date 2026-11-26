using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Tipax.Services
{
    public interface ITipaxService
    {
        Task<List<ShippingOption>> PrepareShippingOptionsAsync(Customer customer, Address address, IList<Product> products);
    }
}
