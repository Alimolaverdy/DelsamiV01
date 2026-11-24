using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.ManufacturerFilter;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Components;

[ViewComponent(Name = "NopAjaxFiltersManufacturerFilters")]
public class ManufacturerFilterComponent : Base7SpikesComponent
{
    private readonly IAclHelper _aclHelper;

    private readonly IManufacturerService7Spikes _manufacturerService7Spikes;

    private readonly IStaticCacheManager _staticCacheManager;

    private readonly IStoreContext _storeContext;

    private readonly IWorkContext _workContext;

    private readonly CatalogSettings _catalogSettings;

    public ManufacturerFilterComponent(IAclHelper aclHelper, IManufacturerService7Spikes manufacturerService7Spikes, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IWorkContext workContext, CatalogSettings catalogSettings)
    {
        _aclHelper = aclHelper;
        _manufacturerService7Spikes = manufacturerService7Spikes;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _catalogSettings = catalogSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync(int categoryId, int vendorId)
    {
        ManufacturerFilterModel7Spikes filterInternalAsync = await this.GetManufacturerFilterInternalAsync(categoryId, vendorId);
        IViewComponentResult result;
        if (filterInternalAsync.ManufacturerFilterItems.Count == 0)
        {
            IViewComponentResult viewComponentResult = Content(string.Empty);
            result = viewComponentResult;
        }
        else
        {
            IViewComponentResult viewComponentResult = View<ManufacturerFilterModel7Spikes>("ManufacturerFilter", filterInternalAsync);
            result = viewComponentResult;
        }
        return result;
    }

    private async Task<ManufacturerFilterModel7Spikes> GetManufacturerFilterInternalAsync(int categoryId, int vendorId)
    {
        IStaticCacheManager staticCacheManager = _staticCacheManager;
        CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_MANUFACTURER_FILTERS_MODEL_KEY;
        object obj1 = categoryId;
        object obj2 = vendorId;
        object obj3 = ((BaseEntity)(await _workContext.GetWorkingLanguageAsync())).Id;
        object obj4 = await _aclHelper.GetAllowedCustomerRolesIdsAsync();
        Store currentStoreAsync = await _storeContext.GetCurrentStoreAsync();
        CacheKey key = staticCacheManager.PrepareKeyForDefaultCache(cacheKey, new object[5]
        {
            obj1,
            obj2,
            obj3,
            obj4,
            currentStoreAsync.Id
        });
        ManufacturerFilterModel7Spikes manufacturerFilterModel = await _staticCacheManager.GetAsync<ManufacturerFilterModel7Spikes>(key, (Func<Task<ManufacturerFilterModel7Spikes>>)async delegate
        {
            IList<Manufacturer> manufacturerList = new List<Manufacturer>();
            if (categoryId > 0)
            {
                manufacturerList = await _manufacturerService7Spikes.GetManufacturersByCategoryIdAsync(categoryId, _catalogSettings.ShowProductsFromSubcategories, false);
            }
            else if (vendorId > 0)
            {
                manufacturerList = await _manufacturerService7Spikes.GetManufacturersByVendorIdAsync(vendorId, false);
            }
            ManufacturerFilterModel7Spikes manufacturerFilterModel7SpikesToReturn = new ManufacturerFilterModel7Spikes();
            if (manufacturerList.Count > 0)
            {
                manufacturerFilterModel7SpikesToReturn = new ManufacturerFilterModel7Spikes
                {
                    CategoryId = categoryId,
                    VendorId = vendorId
                };
                foreach (Manufacturer manufacturer in manufacturerList)
                {
                    ManufacturerFilterItem manufacturerFilterItem1 = new ManufacturerFilterItem
                    {
                        Id = manufacturer.Id
                    };
                    ManufacturerFilterItem manufacturerFilterItem2 = manufacturerFilterItem1;
                    ILocalizationService localizationService = LocalizationService;
                    Manufacturer entity = manufacturer;
                    Expression<Func<Manufacturer, string>> keySelector = (Manufacturer x) => x.Name;
                    int? languageId = null;
                    ManufacturerFilterItem manufacturerFilterItem3 = manufacturerFilterItem2;
                    manufacturerFilterItem3.Name = await localizationService.GetLocalizedAsync<Manufacturer, string>(entity, keySelector, languageId, true, true);
                    manufacturerFilterModel7SpikesToReturn.ManufacturerFilterItems.Add(manufacturerFilterItem1);
                }
            }
            return manufacturerFilterModel7SpikesToReturn;
        });
        return (manufacturerFilterModel.CategoryId != 0 || manufacturerFilterModel.VendorId != 0) ? manufacturerFilterModel : new ManufacturerFilterModel7Spikes();
    }
}
