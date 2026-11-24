using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ZarIran.Plugin.Misc.Core.Enums;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.Products;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.VirtualProducts;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Services;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Factories
{
    public class ZarIranProductModelFactory : ProductModelFactory, IProductModelFactory, IZarIranProductModelFactory
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ICustomZarIranProductService _customProductService;
        private readonly IVirtualProductMappingService _virtualProductService;
        private readonly IWorkContext _workContext;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductTagService _productTagService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ILocalizationService _localizationService;
        private readonly TaxSettings _taxSettings;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IPictureService _pictureService;
        private readonly VendorSettings _vendorSettings;
        private readonly NopHttpClient _nopHttpClient;
        private readonly ISettingService _settingService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IOtherBrandProductService _otherBrandProductService;
        private readonly IBundleProductService _bundleProductService;
        private readonly IProductPricingService _productPricingService;

        #endregion

        #region Ctor

        public ZarIranProductModelFactory(CatalogSettings catalogSettings,
                                           CurrencySettings currencySettings,
                                           IAclSupportedModelFactory aclSupportedModelFactory,
                                           IAddressService addressService,
                                           IBaseAdminModelFactory baseAdminModelFactory,
                                           ICategoryService categoryService,
                                           IOtherBrandProductService otherBrandProductService,
                                           ICustomZarIranProductService customProductService,
                                           ICurrencyService currencyService,
                                           ICustomerService customerService,
                                           IBundleProductService bundleProductService,
                                           IDateTimeHelper dateTimeHelper,
                                           IDiscountService discountService,
                                           IDiscountSupportedModelFactory discountSupportedModelFactory,
                                           ILocalizationService localizationService,
                                           ILocalizedModelFactory localizedModelFactory,
                                           IManufacturerService manufacturerService,
                                           IMeasureService measureService,
                                           IOrderService orderService,
                                           IPictureService pictureService,
                                           IProductAttributeFormatter productAttributeFormatter,
                                           IProductAttributeParser productAttributeParser,
                                           IProductAttributeService productAttributeService,
                                           IProductService productService,
                                           IProductTagService productTagService,
                                           IProductTemplateService productTemplateService,
                                           ISettingModelFactory settingModelFactory,
                                           ISettingService settingService,
                                           IShipmentService shipmentService,
                                           IShippingService shippingService,
                                           IShoppingCartService shoppingCartService,
                                           ISpecificationAttributeService specificationAttributeService,
                                           IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
                                           IStoreContext storeContext,
                                           IStoreService storeService,
                                           IUrlRecordService urlRecordService,
                                           IVideoService videoService,
                                           IWorkContext workContext,
                                           MeasureSettings measureSettings,
                                           NopHttpClient nopHttpClient,
                                           TaxSettings taxSettings,
                                           VendorSettings vendorSettings,
                                           IProductPricingService productPricingService,
                                           IVirtualProductMappingService virtualProductService) : base(catalogSettings,
                                                                                 currencySettings,
                                                                                 aclSupportedModelFactory,
                                                                                 addressService,
                                                                                 baseAdminModelFactory,
                                                                                 categoryService,
                                                                                 currencyService,
                                                                                 customerService,
                                                                                 dateTimeHelper,
                                                                                 discountService,
                                                                                 discountSupportedModelFactory,
                                                                                 localizationService,
                                                                                 localizedModelFactory,
                                                                                 manufacturerService,
                                                                                 measureService,
                                                                                 orderService,
                                                                                 pictureService,
                                                                                 productAttributeFormatter,
                                                                                 productAttributeParser,
                                                                                 productAttributeService,
                                                                                 productService,
                                                                                 productTagService,
                                                                                 productTemplateService,
                                                                                 settingModelFactory,
                                                                                 settingService,
                                                                                 shipmentService,
                                                                                 shippingService,
                                                                                 shoppingCartService,
                                                                                 specificationAttributeService,
                                                                                 storeMappingSupportedModelFactory,
                                                                                 storeContext,
                                                                                 storeService,
                                                                                 urlRecordService,
                                                                                 videoService,
                                                                                 workContext,
                                                                                 measureSettings,
                                                                                 nopHttpClient,
                                                                                 taxSettings,
                                                                                 vendorSettings)
        {
            _productService = productService;
            _customProductService = customProductService;
            _virtualProductService = virtualProductService;
            _workContext = workContext;
            _baseAdminModelFactory = baseAdminModelFactory;
            _urlRecordService = urlRecordService;
            _productTagService = productTagService;
            _productAttributeService = productAttributeService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _localizationService = localizationService;
            _taxSettings = taxSettings;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _bundleProductService = bundleProductService;
            _specificationAttributeService = specificationAttributeService;
            _localizedModelFactory = localizedModelFactory;
            _settingModelFactory = settingModelFactory;
            _productTemplateService = productTemplateService;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _pictureService = pictureService;
            _vendorSettings = vendorSettings;
            _nopHttpClient = nopHttpClient;
            _settingService = settingService;
            _catalogSettings = catalogSettings;
            _otherBrandProductService = otherBrandProductService;
            _productPricingService = productPricingService;
        }

        #endregion

        #region Methods

        public virtual async Task<ZarIranProductSearchModel> PrepareProductSearchModelAsync(ZarIranProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            searchModel.AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts;

            var licenseCheckModel = new LicenseCheckModel();
            try
            {
                var result = await _nopHttpClient.GetLicenseCheckDetailsAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
                    if (licenseCheckModel.DisplayWarning == false && licenseCheckModel.BlockPages == false)
                        await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", false);
                }
            }
            catch { }
            searchModel.LicenseCheckModel = licenseCheckModel;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await PrepareProductType(searchModel.AvailableProductTypes);

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly")
            });

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual async Task<ProductListModel> PrepareProductListModelAsync(ZarIranProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (searchModel.AdvancedSearch)
            {
                //get parameters to filter comments
                var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                if (currentVendor != null)
                    searchModel.SearchVendorId = currentVendor.Id;
                var categoryIds = new List<int> { searchModel.SearchCategoryId };
                if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
                {
                    var childCategoryIds = await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
                    categoryIds.AddRange(childCategoryIds);
                }

                var products = await _productService.SearchProductsAsync(
                    categoryIds: categoryIds,
                    manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                    storeId: 0,
                    vendorId: searchModel.SearchVendorId,
                    warehouseId: 0,
                    productType: searchModel.SearchProductTypeId == 0
                        ? null
                        : (ProductType?)searchModel.SearchProductTypeId,
                    visibleIndividuallyOnly: false,
                    excludeFeaturedProducts: false,
                    keywords: searchModel.SearchProductName,
                    searchDescriptions: false,
                    searchSku: false,
                    searchProductTags: false,
                    languageId: 0,
                    orderBy: ProductSortingEnum.Position,
                    pageIndex: searchModel.Page - 1,
                    pageSize: searchModel.PageSize,
                    showHidden: overridePublished == false
                );

                var pagedProducts = new PagedList<Product>(products, searchModel.Page - 1, searchModel.PageSize, (int)products.TotalCount);

                //prepare list model
                var model = await new ProductListModel().PrepareToGridAsync(searchModel, pagedProducts, () =>
                {
                    return products.SelectAwait(async product =>
                    {
                        //fill in model values from the entity
                        var productModel = product.ToModel<ProductModel>();

                        //little performance optimization: ensure that "FullDescription" is not returned
                        productModel.FullDescription = string.Empty;

                        //fill in additional values (not existing in the entity)
                        productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
                        var defaultProductPicture = (await _pictureService.GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
                        (productModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultProductPicture, 75);
                        productModel.ProductTypeName = await _localizationService.GetLocalizedEnumAsync(product.ProductType);
                        if (product.ProductType == ProductType.SimpleProduct && product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                            productModel.StockQuantityStr = (await _productService.GetTotalStockQuantityAsync(product)).ToString();

                        return productModel;
                    });
                });

                return model;
            }
            else
            {
                //get parameters to filter comments
                var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                if (currentVendor != null)
                    searchModel.SearchVendorId = currentVendor.Id;
                var categoryIds = new List<int> { searchModel.SearchCategoryId };
                if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
                {
                    var childCategoryIds = await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
                    categoryIds.AddRange(childCategoryIds);
                }

                //get products
                var products = await _customProductService.SearchProductsAsync(showHidden: true,
                    categoryIds: categoryIds,
                    manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                    storeId: searchModel.SearchStoreId,
                    vendorId: searchModel.SearchVendorId,
                    warehouseId: searchModel.SearchWarehouseId,
                    productType: searchModel.SearchProductTypeId > 0 ? (ProductTypes)searchModel.SearchProductTypeId : null,
                    keywords: searchModel.SearchProductName,
                    pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                    overridePublished: overridePublished);

                //prepare list model
                var model = await new ProductListModel().PrepareToGridAsync(searchModel, products, () =>
                {
                    return products.SelectAwait(async product =>
                    {
                        //fill in model values from the entity
                        var productModel = product.ToModel<ProductModel>();

                        //little performance optimization: ensure that "FullDescription" is not returned
                        productModel.FullDescription = string.Empty;

                        //fill in additional values (not existing in the entity)
                        productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
                        var defaultProductPicture = (await _pictureService.GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
                        (productModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultProductPicture, 75);
                        productModel.ProductTypeName = await _localizationService.GetLocalizedEnumAsync(product.ProductType);
                        if (product.ProductType == ProductType.SimpleProduct && product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                            productModel.StockQuantityStr = (await _productService.GetTotalStockQuantityAsync(product)).ToString();

                        return productModel;
                    });
                });

                return model;
            }
        }

        public virtual async Task<ZarIranProductListModel> PrepareZarIranProductListModelAsync(ProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;
            var categoryIds = new List<int> { searchModel.SearchCategoryId };
            if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
            {
                var childCategoryIds = await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
                categoryIds.AddRange(childCategoryIds);
            }

            //get products
            var products = await _productService.SearchProductsAsync(showHidden: true,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                warehouseId: searchModel.SearchWarehouseId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: overridePublished);

            //prepare list model
            var model = await new ZarIranProductListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    //fill in model values from the entity
                    var productModel = product.ToModel<ZarIranProductModel>();

                    //little performance optimization: ensure that "FullDescription" is not returned
                    productModel.FullDescription = string.Empty;

                    //fill in additional values (not existing in the entity)
                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
                    var defaultProductPicture = (await _pictureService.GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
                    (productModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultProductPicture, 75);
                    productModel.ProductTypeName = await _localizationService.GetLocalizedEnumAsync(product.ProductType);
                    if (product.ProductType == ProductType.SimpleProduct && product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        productModel.StockQuantityStr = (await _productService.GetTotalStockQuantityAsync(product)).ToString();

                    if ((await _virtualProductService.GetVirtualProductMappingByVirtualProductIdAsync(product.Id)) is not null)
                    {
                        productModel.IsVirtualProduct = true;
                    }

                    return productModel;
                });
            });

            return model;
        }

        public async Task<ZarIranProductModel> PrepareZarIranProductModelAsync(ZarIranProductModel model, Product product, bool excludeProperties = false)
        {
            Func<ProductLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (product != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = product.ToModel<ZarIranProductModel>();

                    var parentProduct = new Product(); 

                    bool isSimpleProduct = product.ProductType == ProductType.SimpleProduct;

                    if (isSimpleProduct)
                        parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

                    if (parentProduct is not null)
                        model.SeName = await _urlRecordService.GetSeNameAsync(isSimpleProduct ? parentProduct : product, 0, true, false);

                    var virtualProductMapping = await _virtualProductService.GetVirtualProductMappingByVirtualProductIdAsync(product.Id);

                    if (virtualProductMapping is not null)
                    {
                        model.IsVirtualProduct = true;

                        if (virtualProductMapping.MainProductId.HasValue)
                        {
                            var mainProduct = await _productService.GetProductByIdAsync(virtualProductMapping.MainProductId.Value);

                            model.MainProductId = virtualProductMapping.MainProductId.Value;
                            model.MainProductName = await _localizationService.GetLocalizedAsync(mainProduct, entity => entity.Name);
                        }
                    }

                }

                var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
                if (parentGroupedProduct != null)
                {
                    model.AssociatedToProductId = product.ParentGroupedProductId;
                    model.AssociatedToProductName = parentGroupedProduct.Name;
                }

                var goldInfo = await _productPricingService.GetByProductIdAsync(product.Id);

                model.MakingFee = goldInfo?.MakingFee ?? 0;
                model.OldMakingFee = goldInfo?.OldMakingFee ?? 0;
                model.ProfitPercent = goldInfo?.ProfitPercent ?? 0;
                model.OldProfitPercent = goldInfo?.OldProfitPercent ?? 0;
                model.CoWorkerProfitPercent = goldInfo?.CoWorkerProfitPercent ?? 0;
                model.OldCoWorkerProfitPercent = goldInfo?.OldCoWorkerProfitPercent ?? 0;
                model.CoWorkerMakingFee = goldInfo?.CoWorkerMakingFee ?? 0;
                model.OldCoWorkerMakingFee = goldInfo?.OldCoWorkerMakingFee ?? 0;
                model.AdditionalAmount = goldInfo?.AdditionalAmount ?? 0;   

                model.LastStockQuantity = product.StockQuantity;
                model.ProductTags = string.Join(", ", (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Select(tag => tag.Name));
                model.ProductAttributesExist = (await _productAttributeService.GetAllProductAttributesAsync()).Any();

                model.CanCreateCombinations = await (await _productAttributeService
                    .GetProductAttributeMappingsByProductIdAsync(product.Id)).AnyAwaitAsync(async pam => (await _productAttributeService.GetProductAttributeValuesAsync(pam.Id)).Any());

                if (!excludeProperties)
                {
                    model.SelectedCategoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true))
                        .Select(productCategory => productCategory.CategoryId).ToList();
                    model.SelectedManufacturerIds = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true))
                        .Select(productManufacturer => productManufacturer.ManufacturerId).ToList();
                }

                //prepare copy product model
                await PrepareCopyProductModelAsync(model.CopyProductModel, product);

                //prepare nested search model
                PrepareRelatedProductSearchModel(model.RelatedProductSearchModel, product);
                PrepareCrossSellProductSearchModel(model.CrossSellProductSearchModel, product);
                PrepareAssociatedProductSearchModel(model.AssociatedProductSearchModel, product);
                PrepareProductPictureSearchModel(model.ProductPictureSearchModel, product);
                PrepareProductVideoSearchModel(model.ProductVideoSearchModel, product);
                PrepareProductSpecificationAttributeSearchModel(model.ProductSpecificationAttributeSearchModel, product);
                PrepareProductOrderSearchModel(model.ProductOrderSearchModel, product);
                PrepareTierPriceSearchModel(model.TierPriceSearchModel, product);
                await PrepareStockQuantityHistorySearchModelAsync(model.StockQuantityHistorySearchModel, product);
                PrepareProductAttributeMappingSearchModel(model.ProductAttributeMappingSearchModel, product);
                PrepareProductAttributeCombinationSearchModel(model.ProductAttributeCombinationSearchModel, product);
                PrepareVirtualProductSearchModel(model.VirtualProductSearchModel, product);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(product, entity => entity.Name, languageId, false, false);
                    locale.FullDescription = await _localizationService.GetLocalizedAsync(product, entity => entity.FullDescription, languageId, false, false);
                    locale.ShortDescription = await _localizationService.GetLocalizedAsync(product, entity => entity.ShortDescription, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(product, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(product, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(product, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(product, languageId, false, false);
                };
            }

            //set default values for the new model
            if (product == null)
            {
                model.MaximumCustomerEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.RecurringCycleLength = 100;
                model.RecurringTotalCycles = 10;
                model.RentalPriceLength = 1;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.OrderMinimumQuantity = 1;
                model.OrderMaximumQuantity = 10000;
                model.TaxCategoryId = _taxSettings.DefaultTaxCategoryId;
                model.UnlimitedDownloads = true;
                model.IsShipEnabled = true;
                model.AllowCustomerReviews = true;
                model.Published = true;
                model.VisibleIndividually = true;
                model.OldMakingFee = 0;
                model.MakingFee = 0;
                model.OldProfitPercent = 0;
                model.ProfitPercent = 0;
                model.CoWorkerProfitPercent = 0;
                model.OldCoWorkerProfitPercent = 0;
                model.CoWorkerMakingFee = 0;
                model.OldCoWorkerMakingFee = 0;

            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;
            model.BaseDimensionIn = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId)).Name;
            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            model.HasAvailableSpecificationAttributes =
                (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync()).Any();

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare editor settings
            model.ProductEditorSettingsModel = await _settingModelFactory.PrepareProductEditorSettingsModelAsync();

            //prepare available product templates
            await _baseAdminModelFactory.PrepareProductTemplatesAsync(model.AvailableProductTemplates, false);

            //prepare available product types
            var productTemplates = await _productTemplateService.GetAllProductTemplatesAsync();
            foreach (var productType in Enum.GetValues(typeof(ProductType)).OfType<ProductType>())
            {
                model.ProductsTypesSupportedByProductTemplates.Add((int)productType, new List<SelectListItem>());
                foreach (var template in productTemplates)
                {
                    var list = (IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredProductTypes) ?? new List<int>();
                    if (string.IsNullOrEmpty(template.IgnoredProductTypes) || !list.Contains((int)productType))
                    {
                        model.ProductsTypesSupportedByProductTemplates[(int)productType].Add(new SelectListItem
                        {
                            Text = template.Name,
                            Value = template.Id.ToString()
                        });
                    }
                }
            }

            //prepare available delivery dates
            await _baseAdminModelFactory.PrepareDeliveryDatesAsync(model.AvailableDeliveryDates,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.DeliveryDate.None"));

            //prepare available product availability ranges
            await _baseAdminModelFactory.PrepareProductAvailabilityRangesAsync(model.AvailableProductAvailabilityRanges,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.ProductAvailabilityRange.None"));

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Vendor.None"));

            //prepare available tax categories
            await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.AvailableTaxCategories);

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(model.AvailableWarehouses,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Warehouse.None"));
            await PrepareProductWarehouseInventoryModelsAsync(model.ProductWarehouseInventoryModels, product);

            //prepare available base price units
            var availableMeasureWeights = (await _measureService.GetAllMeasureWeightsAsync())
                .Select(weight => new SelectListItem { Text = weight.Name, Value = weight.Id.ToString() }).ToList();
            model.AvailableBasepriceUnits = availableMeasureWeights;
            model.AvailableBasepriceBaseUnits = availableMeasureWeights;

            //prepare model categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories, false);
            foreach (var categoryItem in model.AvailableCategories)
            {
                categoryItem.Selected = int.TryParse(categoryItem.Value, out var categoryId)
                    && model.SelectedCategoryIds.Contains(categoryId);
            }

            //prepare model manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers, false);
            foreach (var manufacturerItem in model.AvailableManufacturers)
            {
                manufacturerItem.Selected = int.TryParse(manufacturerItem.Value, out var manufacturerId)
                    && model.SelectedManufacturerIds.Contains(manufacturerId);
            }

            //prepare model discounts
            var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);
            await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, product, availableDiscounts, excludeProperties);

            //prepare model customer roles
            await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(model, product, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, product, excludeProperties);

            var productTags = await _productTagService.GetAllProductTagsAsync();
            var productTagsSb = new StringBuilder();
            productTagsSb.Append("var initialProductTags = [");
            for (var i = 0; i < productTags.Count; i++)
            {
                var tag = productTags[i];
                productTagsSb.Append('\'');
                productTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
                productTagsSb.Append('\'');
                if (i != productTags.Count - 1)
                    productTagsSb.Append(',');
            }
            productTagsSb.Append(']');

            model.InitialProductTags = productTagsSb.ToString();

            return model;
        }

        public virtual async Task<AddVirtualProductSearchModel> PrepareAddVirtualProductSearchModelAsync(AddVirtualProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public virtual async Task<VirtualProductListModel> PrepareVirtualProductListModelAsync(VirtualProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get virtual products
            var virtualProducts = (await _virtualProductService
                .GetVirtualProductMappingsByProductIdAsync(mainProductId: product.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new VirtualProductListModel().PrepareToGridAsync(searchModel, virtualProducts, () =>
            {
                return virtualProducts.SelectAwait(async virtualProduct =>
                {
                    //fill in model values from the entity
                    var virtualProductModel = new VirtualProductModel
                    {
                        Id = virtualProduct.Id,
                        ProductId = virtualProduct.VirtualProductId
                    };

                    //fill in additional values (not existing in the entity)
                    virtualProductModel.ProductName = (await _productService.GetProductByIdAsync(virtualProduct.VirtualProductId))?.Name;

                    return virtualProductModel;
                });
            });

            return model;
        }

        public virtual async Task<AddVirtualProductListModel> PrepareAddVirtualProductListModelAsync(AddVirtualProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get products
            var products = await _virtualProductService.GetNullMainVirtualProductsAsync(categoryIds: new List<int> { searchModel.SearchCategoryId },
                                                                                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                                                                                storeId: searchModel.SearchStoreId,
                                                                                vendorId: searchModel.SearchVendorId,
                                                                                keywords: searchModel.SearchProductName,
                                                                                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                                                                                hasMainProduct: false);

            //prepare grid model
            var model = await new AddVirtualProductListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ProductModel>();

                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        private VirtualProductSearchModel PrepareVirtualProductSearchModel(VirtualProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            searchModel.ProductId = product.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        private async Task PrepareProductType(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available product types
            var availableProductTypeItems = await ProductTypes.SimpleProduct.ToSelectListAsync(false);
            foreach (var productTypeItem in availableProductTypeItems)
            {
                items.Add(productTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItems(items, withSpecialDefaultItem, defaultItemText);
        }

        private async Task PrepareDefaultItems(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
        }

        public async Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int productId, int? specificationId, int specAttribId, bool isSelected = true)
        {
            if (!specificationId.HasValue)
            {
                return new AddSpecificationAttributeModel
                {
                    AvailableAttributes = (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                        .Select(attributeWithOption =>
                        {
                            var attributeName = attributeWithOption.Name;

                            return new SelectListItem(attributeName, attributeWithOption.Id.ToString());
                        }).ToList(),
                    ProductId = productId,
                    Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<AddSpecificationAttributeLocalizedModel>()
                };
            }

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _productService.GetProductByIdAsync(productId)).VendorId != currentVendor.Id)
                throw new UnauthorizedAccessException("This is not your product");

            ProductSpecificationAttribute attribute = new();
            if (isSelected)
            {
                attribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(specificationId.Value);

                if (attribute == null)
                    throw new ArgumentException("No specification attribute found with the specified id");

            }

            var specAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(isSelected ? attribute.SpecificationAttributeOptionId : specificationId.Value);

            var specAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(isSelected ? specAttributeOption.SpecificationAttributeId : specAttribId);

            var model = attribute.ToModel<AddSpecificationAttributeModel>();
            model.SpecificationId = attribute.Id == 0 ? -1 : attribute.Id;
            model.AttributeId = specAttribute.Id;
            model.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);
            model.AttributeName = specAttribute.Name;

            model.AvailableAttributes = (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                .Select(attributeWithOption =>
                {
                    var attributeName = attributeWithOption.Name;

                    return new SelectListItem(attributeName, attributeWithOption.Id.ToString());
                })
                .ToList();

            model.AvailableOptions = (await _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(model.AttributeId))
                .Select(option => new SelectListItem { Text = option.Name, Value = option.Id.ToString() })
                .ToList();
            model.AvailableOptions.Insert(0, new SelectListItem() { Selected = true, Text = "انتخاب نشده", Value = (-1).ToString() });


            switch (attribute.AttributeType)
            {
                case SpecificationAttributeType.Option:
                    model.ValueRaw = WebUtility.HtmlEncode(isSelected ? specAttributeOption.Name : "انتخاب نشده");
                    model.SpecificationAttributeOptionId = isSelected ? specAttributeOption.Id : -1;
                    break;
                case SpecificationAttributeType.CustomText:
                    model.Value = WebUtility.HtmlDecode(attribute.CustomValue);
                    break;
                case SpecificationAttributeType.CustomHtmlText:
                    model.ValueRaw = attribute.CustomValue;
                    break;
                case SpecificationAttributeType.Hyperlink:
                    model.Value = attribute.CustomValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attribute.AttributeType));
            }

            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(
                async (AddSpecificationAttributeLocalizedModel locale, int languageId) =>
                {
                    switch (attribute.AttributeType)
                    {
                        case SpecificationAttributeType.CustomHtmlText:
                            locale.ValueRaw = await _localizationService.GetLocalizedAsync(attribute, entity => entity.CustomValue, languageId, false, false);
                            break;
                        case SpecificationAttributeType.CustomText:
                            locale.Value = await _localizationService.GetLocalizedAsync(attribute, entity => entity.CustomValue, languageId, false, false);
                            break;
                        case SpecificationAttributeType.Option:
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

            return model;
        }

        public async Task<OtherBrandProductListModel> PrepareOtherBrandProductListModelAsync(OtherBrandProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get OtherBrand products
            var otherBrandProducts = (await _otherBrandProductService.GetOtherBrandProductsByProductId1Async(productId1: product.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new OtherBrandProductListModel().PrepareToGridAsync(searchModel, otherBrandProducts, () =>
            {
                return otherBrandProducts.SelectAwait(async otherBrandProduct =>
                {
                    //fill in model values from the entity
                    var otherBrandProductModel = otherBrandProduct.ToModel<OtherBrandProductModel>();

                    //fill in additional values (not existing in the entity)
                    otherBrandProductModel.Product2Name = (await _productService.GetProductByIdAsync(otherBrandProduct.ProductId2))?.Name;

                    return otherBrandProductModel;
                });
            });
            return model;
        }

        public async Task<AddOtherBrandProductSearchModel> PrepareAddOtherBrandProductSearchModelAsync(AddOtherBrandProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public async Task<AddOtherBrandProductListModel> PrepareAddOtherBrandProductListModelAsync(AddOtherBrandProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get products
            var products = await _productService.SearchProductsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddOtherBrandProductListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ZarIranProductModel>();

                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        public async Task<BundleProductListModel> PrepareBundleProductListModelAsync(BundleProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get Bundle products
            var bundleProducts = (await _bundleProductService.GetBundleProductsByProductId1Async(productId1: product.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new BundleProductListModel().PrepareToGridAsync(searchModel, bundleProducts, () =>
            {
                return bundleProducts.SelectAwait(async bundleProduct =>
                {
                    //fill in model values from the entity
                    var bundleProductModel = bundleProduct.ToModel<BundleProductModel>();

                    //fill in additional values (not existing in the entity)
                    bundleProductModel.Product2Name = (await _productService.GetProductByIdAsync(bundleProduct.ProductId2))?.Name;

                    return bundleProductModel;
                });
            });
            return model;
        }

        public async Task<AddBundleProductSearchModel> PrepareAddBundleProductSearchModelAsync(AddBundleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public async Task<AddBundleProductListModel> PrepareAddBundleProductListModelAsync(AddBundleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get products
            var products = await _productService.SearchProductsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddBundleProductListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ZarIranProductModel>();

                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;

        }

        #endregion
    }
}
