using Delsami.Plugin.Misc.Core.Domain;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services
{
    public class VirtualProductMappingService : IVirtualProductMappingService
    {
        #region Fields

        private readonly IRepository<VirtualProductMapping> _virtualProductMappingRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductCategory> _productCategoryRepo;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepo;
        private readonly IRepository<LocalizedProperty> _localizedPropertyRepo;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public VirtualProductMappingService(IRepository<VirtualProductMapping> virtualProductMappingRepo,
                                            IRepository<Product> productRepo,
                                            IRepository<ProductCategory> productCategoryRepo,
                                            IRepository<ProductManufacturer> productManufacturerRepo,
                                            IRepository<LocalizedProperty> localizedPropertyRepo,
                                            IWorkContext workContext)
        {
            _virtualProductMappingRepo = virtualProductMappingRepo;
            _productRepo = productRepo;
            _productCategoryRepo = productCategoryRepo;
            _productManufacturerRepo = productManufacturerRepo;
            _localizedPropertyRepo = localizedPropertyRepo;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        // Refactor and Combine With GetNullMainVirtualProductsAsync ASAP !
        public async Task<IPagedList<Product>> GetVirtualProductsAsync(int pageIndex = 0,
                                                                       int pageSize = int.MaxValue,
                                                                       IList<int> categoryIds = null,
                                                                       IList<int> manufacturerIds = null,
                                                                       int storeId = 0,
                                                                       int vendorId = 0,
                                                                       string keywords = null,
                                                                       bool? hasMainProduct = null)
        {
            var joinQuery = from p in _productRepo.Table
                            join vp in _virtualProductMappingRepo.Table on p.Id equals vp.VirtualProductId
                            where vp.MainProductId != null && !p.Deleted && !vp.Deleted
                            select new { p, vp };


            if (hasMainProduct.HasValue)
            {
                if (hasMainProduct.Value)
                    joinQuery = joinQuery.Where(x => x.vp.MainProductId != null);
                else
                    joinQuery = joinQuery.Where(x => x.vp.MainProductId == null);
            }

            var query = joinQuery.Select(s => s.p);

            if (!string.IsNullOrEmpty(keywords))
                query = from p in query where p.Name.Contains(keywords) select p;

            if (categoryIds is not null)
            {
                if (categoryIds.Contains(0))
                    categoryIds.Remove(0);

                if (categoryIds.Any())
                {
                    var productCategoryQuery =
                        from pc in _productCategoryRepo.Table
                        where !pc.IsFeaturedProduct &&
                            categoryIds.Contains(pc.CategoryId)
                        group pc by pc.ProductId into pc
                        select new
                        {
                            ProductId = pc.Key,
                            pc.First().DisplayOrder
                        };

                    query =
                        from p in query
                        join pc in productCategoryQuery on p.Id equals pc.ProductId
                        orderby pc.DisplayOrder, p.Name
                        select p;
                }
            }

            if (manufacturerIds is not null)
            {
                if (manufacturerIds.Contains(0))
                    manufacturerIds.Remove(0);

                if (manufacturerIds.Any())
                {
                    var productManufacturerQuery =
                        from pm in _productManufacturerRepo.Table
                        where !pm.IsFeaturedProduct &&
                            manufacturerIds.Contains(pm.ManufacturerId)
                        group pm by pm.ProductId into pm
                        select new
                        {
                            ProductId = pm.Key,
                            pm.First().DisplayOrder
                        };

                    query =
                        from p in query
                        join pm in productManufacturerQuery on p.Id equals pm.ProductId
                        orderby pm.DisplayOrder, p.Name
                        select p;
                }
            }

            return await query.OrderBy(_localizedPropertyRepo, await _workContext.GetWorkingLanguageAsync(), ProductSortingEnum.Position).ToPagedListAsync(pageIndex, pageSize);

        }

        public async Task<IPagedList<Product>> GetNullMainVirtualProductsAsync(int pageIndex = 0, int pageSize = int.MaxValue, IList<int> categoryIds = null, IList<int> manufacturerIds = null, int storeId = 0, int vendorId = 0, string keywords = null, bool? hasMainProduct = null)
        {
            var joinQuery = from p in _productRepo.Table
                            join vp in _virtualProductMappingRepo.Table on p.Id equals vp.VirtualProductId
                            where !p.Deleted && !vp.Deleted
                            select new { p, vp };


            if (hasMainProduct.HasValue)
            {
                if (hasMainProduct.Value)
                    joinQuery = joinQuery.Where(x => x.vp.MainProductId != null);
                else
                    joinQuery = joinQuery.Where(x => x.vp.MainProductId == null);
            }

            var query = joinQuery.Select(s => s.p);

            if (!string.IsNullOrEmpty(keywords))
                query = from p in query where p.Name.Contains(keywords) select p;

            if (categoryIds is not null)
            {
                if (categoryIds.Contains(0))
                    categoryIds.Remove(0);

                if (categoryIds.Any())
                {
                    var productCategoryQuery =
                        from pc in _productCategoryRepo.Table
                        where !pc.IsFeaturedProduct &&
                            categoryIds.Contains(pc.CategoryId)
                        group pc by pc.ProductId into pc
                        select new
                        {
                            ProductId = pc.Key,
                            pc.First().DisplayOrder
                        };

                    query =
                        from p in query
                        join pc in productCategoryQuery on p.Id equals pc.ProductId
                        orderby pc.DisplayOrder, p.Name
                        select p;
                }
            }

            if (manufacturerIds is not null)
            {
                if (manufacturerIds.Contains(0))
                    manufacturerIds.Remove(0);

                if (manufacturerIds.Any())
                {
                    var productManufacturerQuery =
                        from pm in _productManufacturerRepo.Table
                        where !pm.IsFeaturedProduct &&
                            manufacturerIds.Contains(pm.ManufacturerId)
                        group pm by pm.ProductId into pm
                        select new
                        {
                            ProductId = pm.Key,
                            pm.First().DisplayOrder
                        };

                    query =
                        from p in query
                        join pm in productManufacturerQuery on p.Id equals pm.ProductId
                        orderby pm.DisplayOrder, p.Name
                        select p;
                }
            }

            return await query.OrderBy(_localizedPropertyRepo, await _workContext.GetWorkingLanguageAsync(), ProductSortingEnum.Position).ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<VirtualProductMapping> GetVirtualProductMappingByVirtualProductIdAsync(int virtualProductId)
        {
            var virtualProductMapping = await _virtualProductMappingRepo.Table.FirstOrDefaultAsync(w => w.VirtualProductId == virtualProductId && !w.Deleted);

            return virtualProductMapping;
        }

        public async Task<List<VirtualProductMapping>> GetVirtualProductMappingsByMainProductIdAsync(int mainProductId)
        {
            var virtualProductMappings = await _virtualProductMappingRepo.Table.Where(w => w.MainProductId == mainProductId && !w.Deleted).ToListAsync();

            return virtualProductMappings;
        }

        public async Task<(bool IsVirtualPrdocut, Product MainProduct)> GetMainProductByVirtualProductIdAsync(int virtualProductId)
        {
            var isVirtualProduct = false;
            Product mainProduct = null;

            var virtualProductMapping = await GetVirtualProductMappingByVirtualProductIdAsync(virtualProductId);

            isVirtualProduct = virtualProductMapping is not null;

            var mainProductId = virtualProductMapping?.MainProductId;

            if (mainProductId.HasValue)
                mainProduct = await _productRepo.GetByIdAsync(mainProductId.Value);

            return (isVirtualProduct, mainProduct);
        }

        public virtual async Task<IList<VirtualProductMapping>> GetVirtualProductMappingsByProductIdAsync(int mainProductId, bool showHidden = false)
        {
            return await GetVirtualProductsByMainProductIdAsync(mainProductId, showHidden);
        }


        public virtual async Task DeleteVirtualProductAsync(VirtualProductMapping virtualProduct)
        {
            virtualProduct.Deleted = true;
            await _virtualProductMappingRepo.UpdateAsync(virtualProduct);
        }

        //public virtual async Task DeleteVirtualProductFromMainAsync(VirtualProductMapping virtualProduct)
        //{
        //    virtualProduct.MainProductId = null;

        //    await _virtualProductMappingRepo.UpdateAsync(virtualProduct);
        //}

        public async Task<VirtualProductMapping> GetVirtualProductAsync(int id)
        {
            return await _virtualProductMappingRepo.Table.FirstOrDefaultAsync(a => a.Id == id && !a.Deleted);
        }

        public VirtualProductMapping FindVirtualProduct(IList<VirtualProductMapping> source, int virtualProductId)
        {
            foreach (var virtualProduct in source)
                if (virtualProduct.VirtualProductId == virtualProductId && !virtualProduct.Deleted)
                    return virtualProduct;

            return null;
        }

        public virtual VirtualProductMapping FindVirtualProduct(IList<VirtualProductMapping> source, int mainProductId, int virtualProductId)
        {
            foreach (var virtualProduct in source)
                if (virtualProduct.MainProductId == mainProductId && virtualProduct.VirtualProductId == virtualProductId && !virtualProduct.Deleted)
                    return virtualProduct;
            return null;
        }

        public async Task InsertVirtualProductAsync(VirtualProductMapping virtualProduct)
        {
            if (virtualProduct is null)
                throw new ArgumentNullException(nameof(virtualProduct));

            virtualProduct.Deleted = false;
            await _virtualProductMappingRepo.InsertAsync(virtualProduct);
        }

        public async Task UpdateVirtualProductAsync(VirtualProductMapping virtualProduct)
        {
            if (virtualProduct is null)
                throw new ArgumentNullException(nameof(virtualProduct));

            await _virtualProductMappingRepo.UpdateAsync(virtualProduct);
        }

        public async Task<IList<VirtualProductMapping>> GetVirtualProductsByMainProductIdAsync(int mainProductId, bool showHidden = false)
        {
            if (mainProductId == 0)
                return new List<VirtualProductMapping>();

            var query = from vp in _virtualProductMappingRepo.Table
                        join p in _productRepo.Table on vp.MainProductId equals p.Id
                        where mainProductId == vp.MainProductId &&
                              !p.Deleted && !vp.Deleted && (showHidden || p.Published)
                        orderby vp.Id
                        select vp;
            var virtualProducts = await query.ToListAsync();

            return virtualProducts;
        }

        public async Task<bool> IsVirtualProductAsync(int productId)
        {
            return await _virtualProductMappingRepo.Table.AnyAsync(a => a.VirtualProductId == productId && !a.Deleted);
        }

        public async Task<bool> IsVirtualProductsAsync(List<int> productIds)
        {
            return await _virtualProductMappingRepo.Table.AnyAsync(a => productIds.Contains(a.VirtualProductId) && !a.Deleted);

        }

        public async Task<bool> IsMainProductAsync(int productId)
        {
            return await _virtualProductMappingRepo.Table.AnyAsync(a => a.MainProductId == productId && !a.Deleted);
        }

        public async Task<Dictionary<int, Product>> GetMainProductsBySimpleProductIdsAsync(List<int> productIds)
        {
            var query = from p in _productRepo.Table.Where(w => !w.Deleted && w.ProductTypeId == (int)ProductType.SimpleProduct && productIds.Contains(w.Id))
                        join vp in _virtualProductMappingRepo.Table.Where(w => !w.Deleted) on p.Id equals vp.VirtualProductId into vpGroup
                        from vp in vpGroup.DefaultIfEmpty()
                        join mp in _productRepo.Table.Where(w => !w.Deleted && w.ProductTypeId == (int)ProductType.SimpleProduct && productIds.Contains(w.Id))
                        on vp.MainProductId equals mp.Id into mpGroup
                        from mp in mpGroup.DefaultIfEmpty()
                        select new
                        {
                            simpleProductId = p.Id,
                            MainProduct = mp ?? p
                        };
            return await query.ToDictionaryAsync(x => x.simpleProductId, d => d.MainProduct);
        }

        #endregion
    }
}
