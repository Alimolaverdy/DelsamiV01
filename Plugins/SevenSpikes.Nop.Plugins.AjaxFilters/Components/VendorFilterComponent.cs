using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.VendorFilter;
using SevenSpikes.Nop.Services.Catalog;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Components;

[ViewComponent(Name = "NopAjaxFiltersVendorFilters")]
public class VendorFilterComponent : Base7SpikesComponent
{
    private readonly IStaticCacheManager _staticCacheManager;

    private readonly IStoreContext _storeContext;

    private readonly IWorkContext _workContext;

    private readonly IVendorService7Spikes _vendorServce;

    private readonly CatalogSettings _catalogSettings;

    public VendorFilterComponent(IStaticCacheManager staticCacheManager, IStoreContext storeContext, IWorkContext workContext, IVendorService7Spikes vendorServce, CatalogSettings catalogSettings)
    {
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _vendorServce = vendorServce;
        _catalogSettings = catalogSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync(int categoryId, int manufacturerId)
    {
        VendorFilterModel7Spikes model = new VendorFilterModel7Spikes();
        if (categoryId > 0)
        {
            model = await this.GetVendorFilterInternalAsync(categoryId);
        }
        else if (manufacturerId > 0)
        {
            model = await this.GetVendorFilterForManufacturerInternalAsync(manufacturerId);
        }
        IViewComponentResult result;
        if (model.VendorFilterItems.Count == 0)
        {
            IViewComponentResult viewComponentResult = Content(string.Empty);
            result = viewComponentResult;
        }
        else
        {
            IViewComponentResult viewComponentResult = View<VendorFilterModel7Spikes>("VendorFilter", model);
            result = viewComponentResult;
        }
        return result;
    }

    private async Task<VendorFilterModel7Spikes> GetVendorFilterInternalAsync(int categoryId)
    {
        IStaticCacheManager staticCacheManager = _staticCacheManager;
        CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_VENDOR_FILTERS_MODEL_KEY;
        object obj1 = categoryId;
        object obj2 = ((BaseEntity)(await _workContext.GetWorkingLanguageAsync())).Id;
        Store currentStoreAsync = await _storeContext.GetCurrentStoreAsync();
        CacheKey key = staticCacheManager.PrepareKeyForDefaultCache(cacheKey, new object[3]
        {
            obj1,
            obj2,
            currentStoreAsync.Id
        });
        VendorFilterModel7Spikes vendorFilterModel = await _staticCacheManager.GetAsync<VendorFilterModel7Spikes>(key, (Func<Task<VendorFilterModel7Spikes>>)async delegate
        {
            IList<Vendor> byCategoryIdAsync = await _vendorServce.GetVendorsByCategoryIdAsync(categoryId, _catalogSettings.ShowProductsFromSubcategories, false);
            VendorFilterModel7Spikes vendorFilterModel7SpikesToReturn = new VendorFilterModel7Spikes();
            if (byCategoryIdAsync.Count > 0)
            {
                vendorFilterModel7SpikesToReturn.CategoryId = categoryId;
                foreach (Vendor vendor in byCategoryIdAsync)
                {
                    VendorFilterItem vendorFilterItem1 = new VendorFilterItem
                    {
                        Id = vendor.Id
                    };
                    VendorFilterItem vendorFilterItem2 = vendorFilterItem1;
                    ILocalizationService localizationService = LocalizationService;
                    Vendor entity = vendor;
                    Expression<Func<Vendor, string>> keySelector = (Vendor v) => v.Name;
                    int? languageId = null;
                    VendorFilterItem vendorFilterItem3 = vendorFilterItem2;
                    vendorFilterItem3.Name = await localizationService.GetLocalizedAsync<Vendor, string>(entity, keySelector, languageId, true, true);
                    vendorFilterModel7SpikesToReturn.VendorFilterItems.Add(vendorFilterItem1);
                }
            }
            return vendorFilterModel7SpikesToReturn;
        });
        return (vendorFilterModel.CategoryId != 0) ? vendorFilterModel : new VendorFilterModel7Spikes();
    }

    private async Task<VendorFilterModel7Spikes> GetVendorFilterForManufacturerInternalAsync(int manufacturerId)
    {
        IList<Vendor> manufacturerIdAsync = await this._vendorServce.GetVendorsByManufacturerIdAsync(manufacturerId, false);
        VendorFilterModel7Spikes vendorFilterModel7SpikesToReturn = new VendorFilterModel7Spikes();
        if (manufacturerIdAsync.Count > 0)
        {
            vendorFilterModel7SpikesToReturn = new VendorFilterModel7Spikes
            {
                CategoryId = 0
            };
            foreach (Vendor vendor in manufacturerIdAsync)
            {
                VendorFilterItem vendorFilterItem1 = new VendorFilterItem
                {
                    Id = vendor.Id
                };
                VendorFilterItem vendorFilterItem2 = vendorFilterItem1;
                ILocalizationService localizationService = LocalizationService;
                Vendor entity = vendor;
                Expression<Func<Vendor, string>> keySelector = (Vendor v) => v.Name;
                int? languageId = null;
                VendorFilterItem vendorFilterItem3 = vendorFilterItem2;
                vendorFilterItem3.Name = await localizationService.GetLocalizedAsync<Vendor, string>(entity, keySelector, languageId, true, true);
                vendorFilterModel7SpikesToReturn.VendorFilterItems.Add(vendorFilterItem1);
            }
        }
        return vendorFilterModel7SpikesToReturn;
    }
}
