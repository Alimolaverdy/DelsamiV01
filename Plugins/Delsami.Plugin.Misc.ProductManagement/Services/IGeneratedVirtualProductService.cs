using Delsami.Plugin.Misc.ProductManagement.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public interface IGeneratedVirtualProductService
    {
        Task InsertAsync(GeneratedVirtualProduct generatedVirtualProduct);

        Task<IList<GeneratedVirtualProduct>> GetAllGeneratedVirtualProductsAsync(bool? publish = null);

        Task UpdateAsync(GeneratedVirtualProduct generatedVirtualProduct);

        Task<bool> IsExistMainParentProductIdAsync(int mainParentProductId);
    }
}
