using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Services
{
    public interface IZarIranLogisticService
    {
        Task<IList<ShippingOption>> PrepareShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest);
    }
}
