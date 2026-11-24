using Microsoft.AspNetCore.Hosting;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.TorobApi.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.TorobApi.Services
{
    public class TorobProductService : ITorobProductService
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IWorkContext _workContext;
        private readonly CoreSettings _coreSettings;
        private readonly IManufacturerService _manufacturerService;

        #endregion

        #region Ctor

        public TorobProductService(IProductService productService,
                                   IWebHostEnvironment env,
                                   ICategoryService categoryService,
                                   IProductAttributeService productAttributeService,
                                   IPictureService pictureService,
                                   MediaSettings mediaSettings,
                                   IStoreContext storeContext,
                                   IUrlRecordService urlRecordService,
                                   ITaxService taxService,
                                   IPriceCalculationService priceCalculationService,
                                   IWorkContext workContext,
                                   CoreSettings coreSettings,
                                   IManufacturerService manufacturerService)
        {
            _productService = productService;
            _env = env;
            _categoryService = categoryService;
            _productAttributeService = productAttributeService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _taxService = taxService;
            _priceCalculationService = priceCalculationService;
            _workContext = workContext;
            _coreSettings = coreSettings;
            _manufacturerService = manufacturerService;
        }

        #endregion

        #region Methods

        public async Task<ProductResponse> GetAllProducts(int limit, int page)
        {
            var products = await _productService.SearchProductsAsync(productType: ProductType.SimpleProduct,
                                                                     pageIndex: page - 1,
                                                                     pageSize: limit);

            //if (showVariations)
            //{
            //    // منطق برای محصولات متغیر
            //    var parentIds = await _context.ProductVariations
            //        .Select(pv => pv.ParentId)
            //        .Distinct()
            //        .ToListAsync();

            //    query = query.Where(p => !parentIds.Contains(p.Id));
            //}
            //else
            //{
            //    query = query.Where(p => p.Type != "variable");
            //}

            //var totalCount = await query.CountAsync();
            //var products = await query
            //    .Skip((page - 1) * limit)
            //    .Take(limit)
            //    .ToListAsync();

            var productDtos = new List<ProductDto>();
            foreach (var product in products)
            {
                var productDto = await MapProductToDto(product);

                if (productDto != null)
                    productDtos.Add(productDto);
            }

            return new ProductResponse
            {
                Count = products.TotalCount,
                Max_Pages = (int)Math.Ceiling(products.TotalCount / (double)limit),
                Products = productDtos
            };
        }

        public async Task<ProductResponse> GetProductsByIds(List<int> productIds)
        {
            var products = await _productService.GetProductsByIdsAsync(productIds.ToArray());

            products = products.Where(w => w.Published).ToList();

            var productDtos = new List<ProductDto>();
            foreach (var product in products)
            {
                var productDto = await MapProductToDto(product);

                if (productDto != null)
                    productDtos.Add(productDto);
            }
            return new ProductResponse
            {
                Count = productDtos.Count,
                Max_Pages = 1,
                Products = productDtos
            };
        }

        public async Task<ProductResponse> GetProductsBySlugs(List<string> slugs)
        {
            slugs = slugs.Distinct().ToList();

            List<UrlRecord> urlRecords = new();

            foreach (var slug in slugs)
            {
                var store = await _storeContext.GetCurrentStoreAsync();

                var pureSlog = slug.Replace($"{store.Url}", "");
                var urlRecord = await _urlRecordService.GetBySlugAsync(pureSlog);

                if (urlRecord != null)
                    urlRecords.Add(urlRecord);
            }

            var productIds = urlRecords.Select(s => s.EntityId).ToList();

            var groupProducts = await _productService.GetProductsByIdsAsync(productIds.ToArray());

            var simpleProducts = new List<Product>();

            foreach (var groupProduct in groupProducts)
            {
                var childProducts = await _productService.GetAssociatedProductsAsync(groupProduct.Id);

                simpleProducts.AddRange(childProducts);
            }

            simpleProducts = simpleProducts.Where(w => w.Published).Distinct().ToList();

            var productDtos = new List<ProductDto>();
            foreach (var product in simpleProducts)
            {
                var productDto = await MapProductToDto(product);

                if (productDto != null)
                    productDtos.Add(productDto);
            }

            return new ProductResponse
            {
                Count = productDtos.Count,
                Max_Pages = 1,
                Products = productDtos
            };
        }

        #endregion

        #region Utilities

        private async Task<ProductDto> MapProductToDto(Product simpleProduct)
        {
            var parentProduct = await _productService.GetProductByIdAsync(simpleProduct.ParentGroupedProductId);
            if (parentProduct == null)
                return null;

            var seName = await _urlRecordService.GetSeNameAsync(parentProduct);
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            decimal finalPriceWithoutDiscountBase;
            decimal finalPriceWithDiscountBase;

            (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(simpleProduct, (await _priceCalculationService.GetFinalPriceAsync(simpleProduct, currentCustomer, currentStore, includeDiscounts: false)).finalPrice);
            (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(simpleProduct, (await _priceCalculationService.GetFinalPriceAsync(simpleProduct, currentCustomer, currentStore, includeDiscounts: true)).finalPrice);


            var dto = new ProductDto
            {
                Title = $"{parentProduct.Name}{await GetManufacturerName(parentProduct.Name, simpleProduct)}",
                Subtitle = "",//parentProduct.ShortDescription,
                //ParentId = product.ParentId ?? 0,
                Page_Unique = simpleProduct.Id.ToString(),
                Current_Price = finalPriceWithDiscountBase.ToString("0.##"),
                Old_Price = finalPriceWithDiscountBase.ToString("0.##"),
                Category_Name = await GetCategoryName(parentProduct.Id),
                Image_Link = (await PrepareImageUrlsAsync(parentProduct.Id, false)).FirstOrDefault(),
                Page_Url = $"{_storeContext.GetCurrentStore().Url}{seName}",
                Short_Desc = parentProduct.ShortDescription,
                //Date = simpleProduct.CreatedOnUtc.ToLocalTime(),
                Registry = "دارد",
                Guarantee = "18 ماه گارانتی شرکتی"
            };

            //if (finalPriceWithDiscountBase != 0 && finalPriceWithDiscountBase < finalPriceWithoutDiscountBase)
            //{
            //    dto.Old_Price = finalPriceWithDiscountBase.ToString("0.##");
            //    dto.Current_Price = finalPriceWithDiscountBase.ToString("0.##");
            //}

            var realProduct = simpleProduct;

            if (realProduct.ManageInventoryMethod == ManageInventoryMethod.DontManageStock)
                dto.Availability = "instock";

            if (realProduct.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
            {
                dto.Availability = realProduct.BackorderMode != BackorderMode.NoBackorders ||
                                   await _productService.GetTotalStockQuantityAsync(realProduct) > 0 ? "instock" : "outofstock";
            }
            if (realProduct.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                var inStock = (await _productAttributeService
                        .GetAllProductAttributeCombinationsAsync(realProduct.Id))
                        ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                        ?? false;

                dto.Availability = inStock ? "instock" : "outofstock";
            }

            dto.Image_Links.AddRange(await PrepareImageUrlsAsync(parentProduct.Id, false));

            return dto;
        }

        //private async Task CalculateVariableProductPrices(Product product, ProductDto dto)
        //{
        //    var variations = await _context.ProductVariations
        //        .Where(pv => pv.ParentId == product.Id)
        //        .ToListAsync();

        //    if (variations.Any())
        //    {
        //        dto.CurrentPrice = variations.Max(v => v.Price);
        //        dto.OldPrice = variations.Max(v => v.RegularPrice);
        //    }
        //}

        private async Task<string> GetCategoryName(int productId)
        {
            var categoryName = string.Empty;

            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);

            if (productCategories.Any())
            {
                var categoryId = productCategories.Where(w => w.CategoryId != _coreSettings.AllProductsCategoryId).FirstOrDefault()?.CategoryId;

                if (categoryId.HasValue)
                {
                    var category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);

                    categoryName = category?.Name;
                }
            }
            return categoryName;
        }

        private async Task<List<string>> PrepareImageUrlsAsync(int productId, bool isAssociatedProduct)
        {
            var imageUrls = new List<string>();
            var pictures = await _pictureService.GetPicturesByProductIdAsync(productId);

            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;

            foreach (var picture in pictures)
            {
                var image = await _pictureService.GetPictureUrlAsync(picture, defaultPictureSize, !isAssociatedProduct);

                imageUrls.Add(image.Url);
            }


            return imageUrls;
        }

        private async Task<string> GetManufacturerName(string parentProductName, Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).FirstOrDefault();

            if (productManufacturer is null)
                return string.Empty;

            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(productManufacturer.ManufacturerId);


            var manufacturerName = manufacturer?.Name;

            if (parentProductName.Contains(manufacturer?.Name))
                return string.Empty;
            else
                return " " + manufacturerName;
        }

        #endregion
    }
}
