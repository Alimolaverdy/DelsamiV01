using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class OtherBrandProductService : IOtherBrandProductService
    {
        #region Fields

        protected readonly IRepository<OtherBrandProduct> _otherBrandProductRepo;
        protected readonly IRepository<Product> _productRepository;
        protected readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public OtherBrandProductService(IRepository<OtherBrandProduct> otherBrandProductRepo, IRepository<Product> productRepository, IStaticCacheManager staticCacheManager)
        {
            _otherBrandProductRepo = otherBrandProductRepo;
            _productRepository = productRepository;
            _staticCacheManager = staticCacheManager;
        }


        #endregion

        #region Methods

        public async Task<IList<OtherBrandProduct>> GetOtherBrandProductsByProductId1Async(int productId1, bool showHidden = false)
        {
            var query = from bmp in _otherBrandProductRepo.Table
                        join p in _productRepository.Table on bmp.ProductId2 equals p.Id
                        where bmp.ProductId1 == productId1 &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby bmp.DisplayOrder, bmp.Id
            select bmp;

            return await query.ToListAsync();
        }


        public async Task<OtherBrandProduct> GetOtherBrandProductByIdAsync(int otherBrandProductId)
        {
            return await _otherBrandProductRepo.GetByIdAsync(otherBrandProductId, cache => default);
        }

        public async Task UpdateOtherBrandProductAsync(OtherBrandProduct otherBrandProduct)
        {
            await _otherBrandProductRepo.UpdateAsync(otherBrandProduct);
        }

        public async Task DeleteOtherBrandProductAsync(OtherBrandProduct otherBrandProduct)
        {
            await _otherBrandProductRepo.DeleteAsync(otherBrandProduct);
        }

        public OtherBrandProduct FindOtherBrandProduct(IList<OtherBrandProduct> source, int productId1, int productId2)
        {
            foreach (var otherBrandProduct in source)
                if (otherBrandProduct.ProductId1 == productId1 && otherBrandProduct.ProductId2 == productId2)
                    return otherBrandProduct;
            return null;
        }

        public async Task InsertOtherBrandProductAsync(OtherBrandProduct otherBrandProduct)
        {
            await _otherBrandProductRepo.InsertAsync(otherBrandProduct);
        }

        #endregion
    }
}
