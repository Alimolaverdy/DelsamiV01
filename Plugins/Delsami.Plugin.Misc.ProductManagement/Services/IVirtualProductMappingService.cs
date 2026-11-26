using Delsami.Plugin.Misc.Core.Domain;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public interface IVirtualProductMappingService
    {
        Task<IPagedList<Product>> GetVirtualProductsAsync(int pageIndex = 0,
                                                          int pageSize = int.MaxValue,
                                                          IList<int> categoryIds = null,
                                                          IList<int> manufacturerIds = null,
                                                          int storeId = 0,
                                                          int vendorId = 0,
                                                          string keywords = null,
                                                          bool? hasMainProduct = null);

        Task<IPagedList<Product>> GetNullMainVirtualProductsAsync(int pageIndex = 0,
                                                                  int pageSize = int.MaxValue,
                                                                  IList<int> categoryIds = null,
                                                                  IList<int> manufacturerIds = null,
                                                                  int storeId = 0,
                                                                  int vendorId = 0,
                                                                  string keywords = null,
                                                                  bool? hasMainProduct = null);

        Task<VirtualProductMapping> GetVirtualProductMappingByVirtualProductIdAsync(int virtualProductId);

        Task<List<VirtualProductMapping>> GetVirtualProductMappingsByMainProductIdAsync(int mainProductId);

        Task<(bool IsVirtualPrdocut, Product MainProduct)> GetMainProductByVirtualProductIdAsync(int virtualProductId);

        Task<IList<VirtualProductMapping>> GetVirtualProductMappingsByProductIdAsync(int mainProductId, bool showHidden = false);

        Task DeleteVirtualProductAsync(VirtualProductMapping virtualProduct);

        //Task DeleteVirtualProductFromMainAsync(VirtualProductMapping virtualProduct);

        Task<VirtualProductMapping> GetVirtualProductAsync(int id);

        VirtualProductMapping FindVirtualProduct(IList<VirtualProductMapping> source, int virtualProductId);

        VirtualProductMapping FindVirtualProduct(IList<VirtualProductMapping> source, int mainProductId, int virtualProductId);

        Task InsertVirtualProductAsync(VirtualProductMapping virtualProduct);

        Task UpdateVirtualProductAsync(VirtualProductMapping virtualProduct);

        Task<bool> IsVirtualProductAsync(int productId);

        Task<bool> IsVirtualProductsAsync(List<int> productIds);

        Task<bool> IsMainProductAsync(int productId);

    }
}
