using ZarIran.Plugin.Misc.ProductManagements.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IGeneratedVirtualProductService
    {
        Task InsertAsync(GeneratedVirtualProduct generatedVirtualProduct);

        Task<IList<GeneratedVirtualProduct>> GetAllGeneratedVirtualProductsAsync(bool? publish = null);

        Task UpdateAsync(GeneratedVirtualProduct generatedVirtualProduct);

        Task<bool> IsExistMainParentProductIdAsync(int mainParentProductId);
    }
}
