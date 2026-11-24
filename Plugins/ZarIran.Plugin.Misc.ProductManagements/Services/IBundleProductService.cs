using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IBundleProductService
    {
        Task<IList<BundleProduct>> GetBundleProductsByProductId1Async(int productId1, bool showHidden = false);

        Task<BundleProduct> GetBundleProductByIdAsync(int bundleProductId);

        Task UpdateBundleProductAsync(BundleProduct bundleProduct);

        Task DeleteBundleProductAsync(BundleProduct bundleProduct);

        BundleProduct FindBundleProduct(IList<BundleProduct> source, int productId1, int productId2);

        Task InsertBundleProductAsync(BundleProduct bundleProduct);

        Task InsertBundleProductsAsync(int productId, IList<(Product Product, int Count)> bundleProducts);

        Task<List<BundleProduct>> GetBundleProductsAsync(bool showHidden = false);

        Task<(List<Product> Products, List<BundleProduct> BundleProducts, List<Category> categories)> GetBundleProductsInludeProductsByProductIdAsync(int productId1);

        Task<List<BundleProduct>> GetBundleProductsAsync(List<int> productId1s);

    }
}
