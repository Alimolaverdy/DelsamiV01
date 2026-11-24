using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Stores;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using SevenSpikes.Nop.Core.Helpers;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.AttributeFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Catalog.DTO;
using SevenSpikes.Nop.Services.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Components;

[ViewComponent(Name = "NopAjaxFiltersAttributeFilters")]
public class AttributeFiltersComponent : Base7SpikesComponent
{
    private readonly IAclHelper _aclHelper;

    private readonly IConvertToDictionaryHelper _convertToDictionaryHelper;

    private readonly IProductAttributeServiceAjaxFilters _productAttributeServiceAjaxFilters;

    private readonly IProductAttributeService7Spikes _productAttributeService7Spikes;

    private readonly IPictureService _pictureService;

    private readonly ISearchQueryStringHelper _searchQueryStringHelper;

    private readonly IStaticCacheManager _staticCacheManager;

    private readonly IStoreContext _storeContext;

    private readonly IWebHelper _webHelper;

    private readonly IWorkContext _workContext;

    private readonly CatalogSettings _catalogSettings;

    private readonly MediaSettings _mediaSettings;

    private readonly NopAjaxFiltersSettings _nopAjaxFiltersSettings;

    private readonly ICustomerService _customerService;

    public AttributeFiltersComponent(IAclHelper aclHelper, IConvertToDictionaryHelper convertToDictionaryHelper, IProductAttributeServiceAjaxFilters productAttributeServiceAjaxFilters, IProductAttributeService7Spikes productAttributeService7Spikes, IPictureService pictureService, ISearchQueryStringHelper searchQueryStringHelper, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IWebHelper webHelper, IWorkContext workContext, CatalogSettings catalogSettings, MediaSettings mediaSettings, NopAjaxFiltersSettings nopAjaxFiltersSettings, ICustomerService customerService)
    {
        _aclHelper = aclHelper;
        _convertToDictionaryHelper = convertToDictionaryHelper;
        _productAttributeServiceAjaxFilters = productAttributeServiceAjaxFilters;
        _productAttributeService7Spikes = productAttributeService7Spikes;
        _pictureService = pictureService;
        _searchQueryStringHelper = searchQueryStringHelper;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
        _catalogSettings = catalogSettings;
        _mediaSettings = mediaSettings;
        _nopAjaxFiltersSettings = nopAjaxFiltersSettings;
        _customerService = customerService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int categoryId, int manufacturerId, int vendorId)
    {
        AttributeFilterModel7Spikes filterInternalAsync = await this.GetProductAttributeFilterInternalAsync(categoryId, manufacturerId, vendorId);
        IViewComponentResult result;
        if (filterInternalAsync.AttributeFilterGroups.Count == 0)
        {
            IViewComponentResult viewComponentResult = Content(string.Empty);
            result = viewComponentResult;
        }
        else
        {
            IViewComponentResult viewComponentResult = View<AttributeFilterModel7Spikes>("AttributeFilter", filterInternalAsync);
            result = viewComponentResult;
        }
        return result;
    }

    private async Task<AttributeFilterModel7Spikes> GetProductAttributeFilterInternalAsync(int categoryId, int manufacturerId, int vendorId)
    {
        SearchQueryStringParameters searchQueryStringParameters = this._searchQueryStringHelper.GetQueryStringParameters(Request.QueryString.Value);
        AttributeFilterModel7Spikes attributeFilterModel7Spikes = new AttributeFilterModel7Spikes();
        if (searchQueryStringParameters.IsOnSearchPage)
        {
            IList<ProductAttribute> productAttributes = new List<ProductAttribute>();
            ICustomerService customerService = this._customerService;
            ICustomerService val = customerService;
            string customerRolesIds = string.Join(",", await val.GetCustomerRoleIdsAsync(await this._workContext.GetCurrentCustomerAsync(), false));
            IStaticCacheManager staticCacheManager = this._staticCacheManager;
            CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_ATTRIBUTE_OPTION_IDS_KEY;
            object obj1 = searchQueryStringParameters.SearchCategoryId;
            object obj3 = searchQueryStringParameters.SearchManufacturerId;
            object obj5 = searchQueryStringParameters.SearchVendorId;
            object obj7 = ((BaseEntity)(await this._workContext.GetWorkingLanguageAsync())).Id;
            object obj9 = customerRolesIds;
            Store currentStoreAsync = await this._storeContext.GetCurrentStoreAsync();
            CacheKey key = staticCacheManager.PrepareKey(cacheKey, new object[12]
            {
                obj1,
                obj3,
                obj5,
                obj7,
                obj9,
                currentStoreAsync.Id,
                searchQueryStringParameters.Keyword,
                searchQueryStringParameters.PriceFrom,
                searchQueryStringParameters.PriceTo,
                searchQueryStringParameters.IncludeSubcategories,
                searchQueryStringParameters.SearchInProductDescriptions,
                searchQueryStringParameters.AdvancedSearch
            });
            IList<int> productAttributeMappingIds = await this._staticCacheManager.GetAsync<IList<int>>(key, (Func<Task<IList<int>>>)(async () => (IList<int>)null));
            if (productAttributeMappingIds != null && productAttributeMappingIds.Any())
            {
                productAttributes = await this._productAttributeService7Spikes.GetAllProductAttributesByProductAttributeMappingIdsAsync(productAttributeMappingIds, false);
            }
            if (productAttributes.Count > 0)
            {
                attributeFilterModel7Spikes = await this.GetAttributeFilterModel7SpikesAsync(categoryId, manufacturerId, vendorId, productAttributes, isOnSearchPage: true, productAttributeMappingIds);
            }
        }
        else
        {
            IStaticCacheManager staticCacheManager = this._staticCacheManager;
            CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_ATTRIBUTE_FILTERS_MODEL_KEY;
            object obj9 = categoryId;
            object obj7 = manufacturerId;
            object obj5 = vendorId;
            object obj3 = ((BaseEntity)(await this._workContext.GetWorkingLanguageAsync())).Id;
            object obj1 = await this._aclHelper.GetAllowedCustomerRolesIdsAsync();
            Store currentStoreAsync2 = await this._storeContext.GetCurrentStoreAsync();
            CacheKey key2 = staticCacheManager.PrepareKeyForDefaultCache(cacheKey, new object[6]
            {
                obj9,
                obj7,
                obj5,
                obj3,
                obj1,
                currentStoreAsync2.Id
            });
            attributeFilterModel7Spikes = await this._staticCacheManager.GetAsync<AttributeFilterModel7Spikes>(key2, (Func<Task<AttributeFilterModel7Spikes>>)async delegate
            {
                IList<ProductAttribute> productAttributesParameter = new List<ProductAttribute>();
                if (categoryId > 0)
                {
                    productAttributesParameter = await _productAttributeService7Spikes.GetAllProductAttributesByCategoryIdAsync(categoryId, _catalogSettings.ShowProductsFromSubcategories, false);
                }
                else if (manufacturerId > 0)
                {
                    productAttributesParameter = await _productAttributeService7Spikes.GetAllProductAttributesByManufacturerIdAsync(manufacturerId, false);
                }
                else if (vendorId > 0)
                {
                    productAttributesParameter = await _productAttributeService7Spikes.GetAllProductAttributesByVendorIdAsync(vendorId, false);
                }
                AttributeFilterModel7Spikes filterInternalAsync = new AttributeFilterModel7Spikes();
                if (productAttributesParameter.Count > 0)
                {
                    filterInternalAsync = await GetAttributeFilterModel7SpikesAsync(categoryId, manufacturerId, vendorId, productAttributesParameter);
                }
                return filterInternalAsync;
            });
        }
        return attributeFilterModel7Spikes;
    }

    private async Task<AttributeFilterModel7Spikes> GetAttributeFilterModel7SpikesAsync(int categoryId, int manufacturerId, int vendorId, IList<ProductAttribute> productAttributesParameter, bool isOnSearchPage = false, IList<int> productAttributeMappingIds = null)
    {
        AttributeFilterModel7Spikes attributeFilterModel7Spikes = new AttributeFilterModel7Spikes
        {
            CategoryId = categoryId,
            ManufacturerId = manufacturerId,
            VendorId = vendorId
        };
        IList<ProductAttributeProductAttributeValueDTO> productAttributeProductAttributeValueDtos = null;
        if (isOnSearchPage && productAttributeMappingIds != null)
        {
            productAttributeProductAttributeValueDtos = await this._productAttributeService7Spikes.GetProductAttributeProductAttributeValueDtosByProductAttributeMappingIdsAsync(productAttributeMappingIds);
        }
        Dictionary<int, AttributeFilterGroup> attributeFilterGroupsDictionary = new Dictionary<int, AttributeFilterGroup>();
        IList<ProductAttribute> source1 = new List<ProductAttribute>(productAttributesParameter);
        IList<string> productAttributes = this._nopAjaxFiltersSettings.ActiveProductAttributes;
        if (productAttributes.Count > 0)
        {
            IList<KeyValuePair<int, ProductAttribute>> source2 = new List<KeyValuePair<int, ProductAttribute>>();
            IDictionary<int, int> semicolonSeparatedPairs = this._convertToDictionaryHelper.CreateDictionaryFromSemicolonSeparatedPairs<int, int>(productAttributes);
            foreach (ProductAttribute productAttribute2 in source1)
            {
                int key = (semicolonSeparatedPairs.ContainsKey(productAttribute2.Id) ? semicolonSeparatedPairs[productAttribute2.Id] : int.MaxValue);
                source2.Add(new KeyValuePair<int, ProductAttribute>(key, productAttribute2));
            }
            source1 = (from x in source2
                orderby x.Key
                select x.Value).ToList();
        }
        if (source1.Count > this._nopAjaxFiltersSettings.NumberOfAttributeFilters)
        {
            source1 = source1.Take(this._nopAjaxFiltersSettings.NumberOfAttributeFilters).ToList();
        }
        foreach (ProductAttribute productAttribute in source1)
        {
            attributeFilterGroupsDictionary.TryGetValue(productAttribute.Id, out var attributeFilterGroup1);
            if (attributeFilterGroup1 == null)
            {
                AttributeFilterGroup attributeFilterGroup4 = new AttributeFilterGroup
                {
                    Id = productAttribute.Id
                };
                AttributeFilterGroup attributeFilterGroup5 = attributeFilterGroup4;
                ILocalizationService localizationService = LocalizationService;
                ProductAttribute entity = productAttribute;
                Expression<Func<ProductAttribute, string>> keySelector = (ProductAttribute x) => x.Name;
                int? languageId = null;
                AttributeFilterGroup attributeFilterGroup6 = attributeFilterGroup5;
                attributeFilterGroup6.Name = await localizationService.GetLocalizedAsync<ProductAttribute, string>(entity, keySelector, languageId, true, true);
                attributeFilterGroup1 = attributeFilterGroup4;
                attributeFilterModel7Spikes.AttributeFilterGroups.Add(attributeFilterGroup1);
                attributeFilterGroupsDictionary.Add(attributeFilterGroup1.Id, attributeFilterGroup1);
            }
            if (isOnSearchPage)
            {
                if (productAttributeProductAttributeValueDtos != null)
                {
                    AttributeFilterGroup attributeFilterGroup4 = attributeFilterGroup1;
                    AttributeFilterGroup attributeFilterGroup6 = attributeFilterGroup4;
                    attributeFilterGroup6.FilterItems = await this.GetAttributeFilterItemsForProductAttributeAsync(productAttribute.Id, productAttributeProductAttributeValueDtos);
                }
            }
            else
            {
                AttributeFilterGroup attributeFilterGroup4 = attributeFilterGroup1;
                AttributeFilterGroup attributeFilterGroup6 = attributeFilterGroup4;
                attributeFilterGroup6.FilterItems = await this.GetAttributeFilterItemsForEntityAndProductAttributeIdAsync(categoryId, manufacturerId, vendorId, productAttribute.Id);
            }
            if (attributeFilterGroup1.FilterItems.Count == 0)
            {
                attributeFilterModel7Spikes.AttributeFilterGroups.Remove(attributeFilterGroup1);
            }
            attributeFilterGroup1 = null;
        }
        return attributeFilterModel7Spikes;
    }

    private async Task<IList<AttributeFilterItem>> GetAttributeFilterItemsForEntityAndProductAttributeIdAsync(int categoryId, int manufacturerId, int vendorId, int productAttributeId)
    {
        IList<ProductAttributeValue> productAttributeValuesLocalized = new List<ProductAttributeValue>();
        if (categoryId > 0)
        {
            productAttributeValuesLocalized = await _productAttributeService7Spikes.GetAllProductVariantAttributeValuesByProductAttributeIdAndCategoryIdAsync(productAttributeId, categoryId, _catalogSettings.ShowProductsFromSubcategories, false);
        }
        else if (manufacturerId > 0)
        {
            productAttributeValuesLocalized = await _productAttributeService7Spikes.GetAllProductVariantAttributeValuesByProductAttributeIdAndManufacturerIdAsync(productAttributeId, manufacturerId, false);
        }
        else if (vendorId > 0)
        {
            productAttributeValuesLocalized = await _productAttributeService7Spikes.GetAllProductVariantAttributeValuesByProductAttributeIdAndVendorIdAsync(productAttributeId, vendorId, false);
        }
        return await PrepareAttributeFilterItemsByProductAttributeValuesAsync(productAttributeValuesLocalized, productAttributeId);
    }

    private async Task<IList<AttributeFilterItem>> GetAttributeFilterItemsForProductAttributeAsync(int productAttributeId, IEnumerable<ProductAttributeProductAttributeValueDTO> productAttributeProductAttributeValueDtos)
    {
        return await PrepareAttributeFilterItemsByProductAttributeValuesAsync(from papav in productAttributeProductAttributeValueDtos
            where papav.ProductAttributeId == productAttributeId
            select papav.ProductAttributeValue, productAttributeId);
    }

    private async Task<IList<AttributeFilterItem>> PrepareAttributeFilterItemsByProductAttributeValuesAsync(
        IEnumerable<ProductAttributeValue> productAttributeValues,
        int productAttributeId)
    {
        var filterItems = new List<AttributeFilterItem>();

        foreach (var attributeValue in productAttributeValues)
        {
            var localizedName = attributeValue.Name.Trim();
            var filterItem = filterItems.FirstOrDefault(item =>
                item.Name.ToLowerInvariant().Trim() == localizedName.ToLowerInvariant());

            if (filterItem == null)
            {
                if (!string.IsNullOrEmpty(attributeValue.ColorSquaresRgb))
                {
                    filterItem = new AttributeFilterItem
                    {
                        ValueId = attributeValue.Id,
                        Name = localizedName,
                        ColorSquaresRgb = attributeValue.ColorSquaresRgb.ToLowerInvariant().Trim()
                    };
                }
                else if (attributeValue.ImageSquaresPictureId > 0)
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                        NopModelCacheDefaults.ProductAttributeImageSquarePictureModelKey,
                        attributeValue.ImageSquaresPictureId,
                        _webHelper.IsCurrentConnectionSecured(),
                        store.Id);

                    var pictureModel = await _staticCacheManager.GetAsync(
                        cacheKey,
                        async () =>
                        {
                            var picture = await _pictureService.GetPictureByIdAsync(
                                attributeValue.ImageSquaresPictureId);
                            if (picture == null)
                                return new PictureModel();

                            return new PictureModel
                            {
                                FullSizeImageUrl = await _pictureService.GetPictureUrlAsync(
                                    picture.Id,
                                    0,
                                    true,
                                    null,
                                    (PictureType)1),
                                ImageUrl = await _pictureService.GetPictureUrlAsync(
                                    picture.Id,
                                    _mediaSettings.ImageSquarePictureSize,
                                    true,
                                    null,
                                    (PictureType)1)
                            };
                        });

                    filterItem = new AttributeFilterItem
                    {
                        ValueId = attributeValue.Id,
                        Name = localizedName,
                        ImageSquaresUrl = pictureModel.ImageUrl
                    };
                }
                else
                {
                    filterItem = new AttributeFilterItem
                    {
                        ValueId = attributeValue.Id,
                        Name = localizedName
                    };
                }

                filterItem.AttributeId = productAttributeId;
                filterItems.Add(filterItem);
            }
            else if (!string.IsNullOrEmpty(attributeValue.ColorSquaresRgb))
            {
                filterItem.ColorSquaresRgb = attributeValue.ColorSquaresRgb.ToLowerInvariant().Trim();
            }

            filterItem.ProductVariantAttributeIds.Add(attributeValue.ProductAttributeMappingId);
        }

        return await _productAttributeServiceAjaxFilters
            .GetSortedAttributeValuesBasedOnTheirPredefinedDisplayOrderAsync(filterItems);
    }

}
