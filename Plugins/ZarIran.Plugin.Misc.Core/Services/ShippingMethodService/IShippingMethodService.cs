
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.Core.Services.ShippingMethodService
{
    public interface IShippingMethodService
    {
        Task<Nop.Core.Domain.Shipping.ShippingMethod> GetShippingMethodByNameAsync(string name);

    }
}
