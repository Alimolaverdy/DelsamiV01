using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain.Enums;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.InStockFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;
using SevenSpikes.Nop.Services.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Components;

[ViewComponent(Name = "NopAjaxFiltersInStockFilter")]
public class InStockFilterComponent : Base7SpikesComponent
{
    private readonly IAclHelper _aclHelper;

    private readonly ILocalizationService _localizationService;

    private readonly IProductServiceNopAjaxFilters _productService7Spikes;

    private readonly IStaticCacheManager _staticCacheManager;

    private readonly IStoreContext _storeContext;

    private readonly IWorkContext _workContext;

    public InStockFilterComponent(IAclHelper aclHelper, ILocalizationService localizationService, IProductServiceNopAjaxFilters productService7Spikes, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IWorkContext workContext)
    {
        _aclHelper = aclHelper;
        _localizationService = localizationService;
        _productService7Spikes = productService7Spikes;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(int categoryId, int manufacturerId, int vendorId)
    {
        InStockFilterModel7Spikes filterInternalAsync = await this.GetInStockFilterInternalAsync(categoryId, manufacturerId, vendorId);
        IViewComponentResult result;
        if (filterInternalAsync == null)
        {
            IViewComponentResult viewComponentResult = Content(string.Empty);
            result = viewComponentResult;
        }
        else
        {
            IViewComponentResult viewComponentResult = View<InStockFilterModel7Spikes>("InStockFilter", filterInternalAsync);
            result = viewComponentResult;
        }
        return result;
    }

    private async Task<InStockFilterModel7Spikes> GetInStockFilterInternalAsync(int categoryId, int manufacturerId, int vendorId)
    {
        IStaticCacheManager staticCacheManager = _staticCacheManager;
        CacheKey cacheKey = NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_INSTOCK_FILTERS_MODEL_KEY;
        object obj1 = ((BaseEntity)(await _workContext.GetWorkingLanguageAsync())).Id;
        object obj2 = await _aclHelper.GetAllowedCustomerRolesIdsAsync();
        Store currentStoreAsync = await _storeContext.GetCurrentStoreAsync();
        CacheKey key = staticCacheManager.PrepareKeyForDefaultCache(cacheKey, new object[6]
        {
            obj1,
            obj2,
            currentStoreAsync.Id,
            categoryId,
            manufacturerId,
            vendorId
        });
        if (!(await _staticCacheManager.GetAsync<bool>(key, (Func<Task<bool>>)(async () => await _productService7Spikes.HasProductsInStockAsync(categoryId, manufacturerId, vendorId)))))
        {
            return null;
        }
        InStockFilterModel7Spikes filterModel7Spikes1 = new InStockFilterModel7Spikes
        {
            Id = 1
        };
        InStockFilterModel7Spikes inStockFilterModel7Spikes = filterModel7Spikes1;
        inStockFilterModel7Spikes.Name = await _localizationService.GetResourceAsync("SevenSpikes.NopAjaxFilters.Public.InStock.Option");
        filterModel7Spikes1.CategoryId = categoryId;
        filterModel7Spikes1.VendorId = vendorId;
        filterModel7Spikes1.ManufacturerId = manufacturerId;
        filterModel7Spikes1.FilterItemState = FilterItemState.Unchecked;
        return filterModel7Spikes1;
    }
}
