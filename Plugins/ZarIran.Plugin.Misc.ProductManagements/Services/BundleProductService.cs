using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Irony.Parsing;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class BundleProductService : IBundleProductService
    {

        #region Fields

        protected readonly IRepository<BundleProduct> _bundleProductRepo;
        protected readonly IRepository<Product> _productRepository;
        protected readonly IRepository<ProductCategory> _productCategoryRepository;
        protected readonly IRepository<Category> _categoryRepository;
        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public BundleProductService(IRepository<BundleProduct> bundleProductRepo, IRepository<Product> productRepository, IStaticCacheManager staticCacheManager, IRepository<ProductCategory> productCategoryRepository, IRepository<Category> categoryRepository)
        {
            _bundleProductRepo = bundleProductRepo;
            _productRepository = productRepository;
            _staticCacheManager = staticCacheManager;
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
        }

        #endregion

        #region Methods

        public async Task DeleteBundleProductAsync(BundleProduct bundleProduct)
        {
            await _bundleProductRepo.DeleteAsync(bundleProduct);
        }

        public BundleProduct FindBundleProduct(IList<BundleProduct> source, int productId1, int productId2)
        {
            foreach (var bundleProduct in source)
                if (bundleProduct.ProductId1 == productId1 && bundleProduct.ProductId2 == productId2)
                    return bundleProduct;
            return null;
        }

        public async Task<BundleProduct> GetBundleProductByIdAsync(int bundleProductId)
        {
            return await _bundleProductRepo.GetByIdAsync(bundleProductId, cache => default);
        }

        public async Task<IList<BundleProduct>> GetBundleProductsByProductId1Async(int productId1, bool showHidden = false)
        {
            var query = from bp in _bundleProductRepo.Table
                        join p in _productRepository.Table on bp.ProductId2 equals p.Id
                        where bp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby bp.Id
                        select bp;

            return await query.ToListAsync();
        }

        public async Task InsertBundleProductAsync(BundleProduct bundleProduct)
        {
            await _bundleProductRepo.InsertAsync(bundleProduct);
        }

        public async Task UpdateBundleProductAsync(BundleProduct bundleProduct)
        {
            await _bundleProductRepo.UpdateAsync(bundleProduct);
        }

        public async Task InsertBundleProductsAsync(int productId, IList<(Product Product, int Count)> bundleProducts)
        {
            foreach (var item in bundleProducts)
            {
                await InsertBundleProductAsync(new BundleProduct
                {
                    ProductId1 = productId,
                    ProductId2 = item.Product.Id,
                    Count = item.Count
                });
            }
        }

        public async Task<List<BundleProduct>> GetBundleProductsAsync(bool showHidden = true)
        {
            var query = from bp in _bundleProductRepo.Table
                        join p in _productRepository.Table on bp.ProductId1 equals p.Id
                        where !p.Deleted &&
                        (showHidden || p.Published)
                        orderby bp.Id
                        select bp;

            return await query.ToListAsync();
        }

        public async Task<(List<Product> Products, List<BundleProduct> BundleProducts, List<Category> categories)> GetBundleProductsInludeProductsByProductIdAsync(int productId1)
        {
            var result = await (from bp in _bundleProductRepo.Table
                                join p in _productRepository.Table on bp.ProductId2 equals p.Id
                                join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId into productCategoryGroup
                                from pc in productCategoryGroup.DefaultIfEmpty()
                                join c in _categoryRepository.Table on pc.CategoryId equals c.Id into categoryGroup
                                from c in categoryGroup.DefaultIfEmpty()
                                where !p.Deleted && bp.ProductId1 == productId1
                                orderby bp.Id
                                select new { Product = p, BundleProduct = bp, Category = c }).ToListAsync();


            var bundleProducts = result
                .Select(s => s.BundleProduct)
                .DistinctBy(d => d.Id)
                .ToList();

            var products = result
                .Select(s => s.Product)
                .DistinctBy(d => d.Id)
                .ToList();

            var categories = result
                .Select(s => s.Category)
                .DistinctBy(d => d.Id)
                .ToList();

            return (products, bundleProducts, categories);
        }

        public async Task<List<BundleProduct>> GetBundleProductsAsync(List<int> productId1s)
        {
            return await _bundleProductRepo.Table.Where(w => productId1s.Contains(w.ProductId1)).ToListAsync();
        }

        #endregion

    }
}
