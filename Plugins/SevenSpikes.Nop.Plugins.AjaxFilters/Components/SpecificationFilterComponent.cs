using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.SpecificationFilter;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Components;

[ViewComponent(Name = "NopAjaxFiltersSpecificationFilters")]
public class SpecificationFilterComponent : Base7SpikesComponent
{
    private readonly IAclHelper _aclHelper;

    private readonly ISearchQueryStringHelper _searchQueryStringHelper;

    private readonly ISpecificationAttributeService7Spikes _specificationAttributeService7Spikes;

    private readonly IStaticCacheManager _staticCacheManager;

    private readonly IStoreContext _storeContext;

    private readonly IWorkContext _workContext;

    private readonly CatalogSettings _catalogSettings;

    private readonly NopAjaxFiltersSettings _nopAjaxFilterSettings;

    private readonly ISpecificationAttributeService _specificationAttributeService;

    public SpecificationFilterComponent(IAclHelper aclHelper, ISearchQueryStringHelper searchQueryStringHelper, ISpecificationAttributeService7Spikes specificationAttributeService7Spikes, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IWorkContext workContext, CatalogSettings catalogSettings, NopAjaxFiltersSettings nopAjaxFilterSettings, ISpecificationAttributeService specificationAttributeService)
    {
        _aclHelper = aclHelper;
        _searchQueryStringHelper = searchQueryStringHelper;
        _specificationAttributeService7Spikes = specificationAttributeService7Spikes;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _catalogSettings = catalogSettings;
        _nopAjaxFilterSettings = nopAjaxFilterSettings;
        _specificationAttributeService = specificationAttributeService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int categoryId, int manufacturerId, int vendorId)
    {
        SpecificationFilterModel7Spikes filterInternalAsync = await this.GetSpecificationFilterInternalAsync(categoryId, manufacturerId, vendorId);
        IViewComponentResult result;
        if (filterInternalAsync.SpecificationFilterGroups.Count == 0)
        {
            IViewComponentResult viewComponentResult = Content(string.Empty);
            result = viewComponentResult;
        }
        else
        {
            IViewComponentResult viewComponentResult = View<SpecificationFilterModel7Spikes>("SpecificationFilter", filterInternalAsync);
            result = viewComponentResult;
        }
        return result;
    }

    private async Task<SpecificationFilterModel7Spikes> GetSpecificationFilterInternalAsync(int categoryId, int manufacturerId, int vendorId)
    {
        SearchQueryStringParameters searchQueryStringParameters = this._searchQueryStringHelper.GetQueryStringParameters(Request.QueryString.Value);
        SpecificationFilterModel7Spikes specificationFilterModel7Spikes = new SpecificationFilterModel7Spikes();
        if (searchQueryStringParameters.IsOnSearchPage)
        {
            ICustomerService customerService = CustomerService;
            ICustomerService val = customerService;
            string customerRolesIds = string.Join(",", await val.GetCustomerRoleIdsAsync(await this._workContext.GetCurrentCustomerAsync(), false));
            IStaticCacheManager staticCacheManager = this._staticCacheManager;
            CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_SPECIFICATION_OPTION_IDS_KEY;
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
            IList<int> cachedProductIds = await this._staticCacheManager.GetAsync<IList<int>>(key, (Func<Task<IList<int>>>)(async () => (IList<int>)null));
            IList<SpecificationAttributeOption> specificationAttributeOptionList = new List<SpecificationAttributeOption>();
            if (cachedProductIds != null && cachedProductIds.Any())
            {
                specificationAttributeOptionList = await this._specificationAttributeService7Spikes.GetSpecificationAttributeOptionsByIdsAsync(cachedProductIds);
            }
            if (specificationAttributeOptionList.Count > 0)
            {
                specificationFilterModel7Spikes = await this.GetSpecificationFilterModel7SpikesAsync(categoryId, manufacturerId, vendorId, specificationAttributeOptionList);
            }
        }
        else
        {
            IStaticCacheManager staticCacheManager = this._staticCacheManager;
            CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_SPECIFICATION_FILTERS_MODEL_KEY;
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
            specificationFilterModel7Spikes = await this._staticCacheManager.GetAsync<SpecificationFilterModel7Spikes>(key2, (Func<Task<SpecificationFilterModel7Spikes>>)async delegate
            {
                IList<SpecificationAttributeOption> specificationAttributeOptionList2 = new List<SpecificationAttributeOption>();
                if (categoryId > 0)
                {
                    specificationAttributeOptionList2 = await _specificationAttributeService7Spikes.GetSpecificationAttributeOptionsByCategoryIdAsync(categoryId, _catalogSettings.ShowProductsFromSubcategories, false);
                }
                else if (manufacturerId > 0)
                {
                    specificationAttributeOptionList2 = await _specificationAttributeService7Spikes.GetSpecificationAttributeOptionsByManufacturerIdAsync(manufacturerId, false);
                }
                else if (vendorId > 0)
                {
                    specificationAttributeOptionList2 = await _specificationAttributeService7Spikes.GetSpecificationAttributeOptionsByVendorIdAsync(vendorId, false);
                }
                SpecificationFilterModel7Spikes filterInternalAsync = new SpecificationFilterModel7Spikes();
                if (specificationAttributeOptionList2.Count > 0)
                {
                    filterInternalAsync = await GetSpecificationFilterModel7SpikesAsync(categoryId, manufacturerId, vendorId, specificationAttributeOptionList2);
                }
                return filterInternalAsync;
            });
        }
        return specificationFilterModel7Spikes;
    }

    private async Task<SpecificationFilterModel7Spikes> GetSpecificationFilterModel7SpikesAsync(int categoryId, int manufacturerId, int vendorId, IEnumerable<SpecificationAttributeOption> specificationAttributeOptions)
    {
        SpecificationFilterModel7Spikes specificationFilterModel7Spikes = new SpecificationFilterModel7Spikes
        {
            CategoryId = categoryId,
            ManufacturerId = manufacturerId,
            VendorId = vendorId
        };
        Dictionary<int, SpecificationFilterGroup> specificationFilterGroupsDictionary = new Dictionary<int, SpecificationFilterGroup>();
        foreach (SpecificationAttributeOption specificationAttributeOption in specificationAttributeOptions)
        {
            specificationFilterGroupsDictionary.TryGetValue(specificationAttributeOption.SpecificationAttributeId, out var specificationFilterGroup);
            if (specificationFilterGroup == null)
            {
                SpecificationAttribute attributeByIdAsync = await this._specificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeOption.SpecificationAttributeId);
                SpecificationFilterGroup specificationFilterGroup2 = new SpecificationFilterGroup
                {
                    Id = specificationAttributeOption.SpecificationAttributeId,
                    DisplayOrder = attributeByIdAsync.DisplayOrder
                };
                SpecificationFilterGroup specificationFilterGroup3 = specificationFilterGroup2;
                ILocalizationService localizationService = LocalizationService;
                SpecificationAttribute entity = attributeByIdAsync;
                Expression<Func<SpecificationAttribute, string>> keySelector = (SpecificationAttribute x) => x.Name;
                int? languageId = null;
                SpecificationFilterGroup specificationFilterGroup4 = specificationFilterGroup3;
                specificationFilterGroup4.Name = await localizationService.GetLocalizedAsync<SpecificationAttribute, string>(entity, keySelector, languageId, true, true);
                specificationFilterGroup = specificationFilterGroup2;
                specificationFilterModel7Spikes.SpecificationFilterGroups.Add(specificationFilterGroup);
                specificationFilterGroupsDictionary.Add(specificationFilterGroup.Id, specificationFilterGroup);
            }
            string colorSquaresRgb = ((!string.IsNullOrEmpty(specificationAttributeOption.ColorSquaresRgb)) ? specificationAttributeOption.ColorSquaresRgb.ToLower().Trim() : null);
            IList<SpecificationFilterItem> specificationFilterItemList = specificationFilterGroup.FilterItems;
            SpecificationFilterItem specificationFilterItem1 = new SpecificationFilterItem
            {
                Id = specificationAttributeOption.Id
            };
            SpecificationFilterItem specificationFilterItem2 = specificationFilterItem1;
            ILocalizationService localizationService2 = LocalizationService;
            SpecificationAttributeOption entity2 = specificationAttributeOption;
            Expression<Func<SpecificationAttributeOption, string>> keySelector2 = (SpecificationAttributeOption x) => x.Name;
            int? languageId2 = null;
            SpecificationFilterItem specificationFilterItem3 = specificationFilterItem2;
            specificationFilterItem3.Name = await localizationService2.GetLocalizedAsync<SpecificationAttributeOption, string>(entity2, keySelector2, languageId2, true, true);
            specificationFilterItem1.DisplayOrder = specificationAttributeOption.DisplayOrder;
            specificationFilterItem1.ColorSquaresRgb = colorSquaresRgb;
            specificationFilterItemList.Add(specificationFilterItem1);
        }
        foreach (SpecificationFilterGroup specificationFilterGroup5 in specificationFilterModel7Spikes.SpecificationFilterGroups)
        {
            specificationFilterGroup5.FilterItems = (from x in specificationFilterGroup5.FilterItems
                                                     orderby x.DisplayOrder, x.Name
                                                     select x).ToList();
        }
        specificationFilterModel7Spikes.SpecificationFilterGroups = specificationFilterModel7Spikes.SpecificationFilterGroups.OrderBy((SpecificationFilterGroup x) => x.DisplayOrder).Take(this._nopAjaxFilterSettings.NumberOfSpecificationFilters).ToList();
        return specificationFilterModel7Spikes;
    }
}
