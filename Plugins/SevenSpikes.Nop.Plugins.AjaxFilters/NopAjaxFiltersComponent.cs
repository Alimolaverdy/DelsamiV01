using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Common;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models;
using SevenSpikes.Nop.Services.Helpers;

[ViewComponent(Name = "NopAjaxFilters")]
public class NopAjaxFiltersComponent : Base7SpikesComponent
{
    private readonly IAclHelper _aclHelper;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly CatalogSettings _catalogSettings;
    private readonly NopAjaxFiltersSettings _settings;
    private readonly IFiltersPageHelper _filtersPageHelper;
    private readonly IGenericAttributeService _genericAttributeService;

    public NopAjaxFiltersComponent(
        IAclHelper aclHelper,
        IStoreContext storeContext,
        IWorkContext workContext,
        IStaticCacheManager staticCacheManager,
        CatalogSettings catalogSettings,
        NopAjaxFiltersSettings settings,
        IFiltersPageHelper filtersPageHelper,
        IGenericAttributeService genericAttributeService)
    {
        _aclHelper = aclHelper;
        _storeContext = storeContext;
        _workContext = workContext;
        _staticCacheManager = staticCacheManager;
        _catalogSettings = catalogSettings;
        _settings = settings;
        _filtersPageHelper = filtersPageHelper;
        _genericAttributeService = genericAttributeService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone)
    {
        if (_settings.WidgetZone != widgetZone || !_settings.EnableAjaxFilters)
            return Content(string.Empty);

        var isFromHomePageFilter = string.Equals(
            Request.Query["fromHomePageFilter"].FirstOrDefault(),
            "true",
            StringComparison.OrdinalIgnoreCase);
        var filtersPageHelper = await GetFiltersPageHelperAsync();
        var pageParameters = filtersPageHelper.GetFiltersPageParameters();

        if (!await filtersPageHelper.ValidateParametersAsync(pageParameters)
            || (!ShouldShowFiltersInCategory(pageParameters.CategoryId) && !isFromHomePageFilter)
            || (pageParameters.ManufacturerId > 0 && !_settings.ShowFiltersOnManufacturerPage)
            || (pageParameters.VendorId > 0 && !_settings.ShowFiltersOnVendorPage)
            || (pageParameters.SearchQueryStringParameters.IsOnSearchPage && !_settings.ShowFiltersOnSearchPage))
        {
            return Content(string.Empty);
        }

        var model = await GetCachedNopAjaxFiltersModelAsync(pageParameters);
        return View("NopFilters", model);
    }

    private bool ShouldShowFiltersInCategory(int categoryId)
    {
        if (!_settings.ShowFiltersOnCategoryPage)
            return false;

        if (categoryId <= 0 || string.IsNullOrEmpty(_settings.CategoriesWithoutFilters))
            return true;

        foreach (var categoryIdText in _settings.CategoriesWithoutFilters.Split(','))
        {
            if (int.TryParse(categoryIdText, out var excludedCategoryId)
                && excludedCategoryId == categoryId)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<NopAjaxFiltersModel> GetCachedNopAjaxFiltersModelAsync(
        FiltersPageParameters pageParameters)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var customerRoleIds = await _aclHelper.GetAllowedCustomerRolesIdsAsync();
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var queryParameters = pageParameters.SearchQueryStringParameters;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_MODEL_KEY,
            pageParameters.CategoryId,
            pageParameters.ManufacturerId,
            pageParameters.VendorId,
            customer.Id,
            languageId,
            customerRoleIds,
            storeId,
            pageParameters.OrderBy,
            pageParameters.ViewMode,
            pageParameters.PageSize,
            queryParameters.Keyword,
            queryParameters.SearchCategoryId,
            queryParameters.SearchManufacturerId,
            queryParameters.SearchVendorId,
            queryParameters.PriceFrom,
            queryParameters.PriceTo,
            queryParameters.IncludeSubcategories,
            queryParameters.SearchInProductDescriptions,
            queryParameters.AdvancedSearch);

        return await _staticCacheManager.GetAsync(
            cacheKey,
            async () =>
            {
                var model = new NopAjaxFiltersModel
                {
                    CategoryId = pageParameters.CategoryId,
                    ManufacturerId = pageParameters.ManufacturerId,
                    VendorId = pageParameters.VendorId,
                    SearchQueryStringParameters = queryParameters,
                    DefaultViewMode = _catalogSettings.DefaultViewMode,
                    AvailableSortOptionsJson = await GetAvailableSortOptionsJsonAsync(pageParameters.OrderBy),
                    AvailableViewModesJson = await GetAvailableViewModesJsonAsync(pageParameters.ViewMode),
                    AvailablePageSizesJson = GetAvailablePageSizesJson(pageParameters.PageSize)
                };

                model.FromCarSelector = await _genericAttributeService.GetAttributeAsync<bool>(
                    customer,
                    "AjaxFilterSearchType.FromCarSelector",
                    0,
                    false);

                return model;
            });
    }

    private async Task<string> GetAvailableSortOptionsJsonAsync(int? selectedOrderBy)
    {
        var allSortOptions = Enum.GetValues<ProductSortingEnum>();
        var allOptionsDisabled = _catalogSettings.ProductSortingEnumDisabled.Count == allSortOptions.Length;

        var sortOptions = allSortOptions
            .Select(option => (int)option)
            .Except(_catalogSettings.ProductSortingEnumDisabled)
            .Select(optionId => new KeyValuePair<int, int>(
                optionId,
                _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(optionId, out var displayOrder)
                    ? displayOrder
                    : optionId))
            .OrderBy(option => option.Value)
            .ToList();

        if (!selectedOrderBy.HasValue)
            selectedOrderBy = allOptionsDisabled ? 0 : sortOptions.First().Key;

        if (!_catalogSettings.AllowProductSorting || allOptionsDisabled)
            return string.Empty;

        var model = new List<SelectListItem>(sortOptions.Count);
        foreach (var option in sortOptions)
        {
            model.Add(new SelectListItem
            {
                Text = await LocalizationService.GetLocalizedEnumAsync<ProductSortingEnum>(
                    (ProductSortingEnum)option.Key,
                    null),
                Value = option.Key.ToString(),
                Selected = option.Key == selectedOrderBy
            });
        }

        return JsonConvert.SerializeObject(model);
    }

    private async Task<string> GetAvailableViewModesJsonAsync(string viewMode)
    {
        if (!_catalogSettings.AllowProductViewModeChanging)
            return string.Empty;

        var selectedViewMode = string.IsNullOrWhiteSpace(viewMode)
            ? _catalogSettings.DefaultViewMode
            : viewMode;
        selectedViewMode = selectedViewMode.ToLowerInvariant();

        if (selectedViewMode != "grid" && selectedViewMode != "list")
            selectedViewMode = "grid";

        var model = new List<SelectListItem>
        {
            new()
            {
                Text = await LocalizationService.GetResourceAsync("Catalog.ViewMode.Grid"),
                Value = "grid",
                Selected = selectedViewMode == "grid"
            },
            new()
            {
                Text = await LocalizationService.GetResourceAsync("Catalog.ViewMode.List"),
                Value = "list",
                Selected = selectedViewMode == "list"
            }
        };

        return JsonConvert.SerializeObject(model);
    }

    private string GetAvailablePageSizesJson(int pageSize)
    {
        var pageSizes = _filtersPageHelper.GetPageSizes();
        if (pageSizes == null || pageSizes.Length == 0)
            return string.Empty;

        if (pageSize <= 0 || !pageSizes.Contains(pageSize.ToString()))
            pageSize = _filtersPageHelper.GetDefaultPageSize();

        var model = pageSizes
            .Where(size => int.TryParse(size, out var parsedSize) && parsedSize > 0)
            .Select(size => new SelectListItem
            {
                Text = size,
                Value = size,
                Selected = string.Equals(
                    size,
                    pageSize.ToString(),
                    StringComparison.InvariantCultureIgnoreCase)
            })
            .OrderBy(item => int.Parse(item.Text))
            .ToList();

        return model.Count == 0 ? string.Empty : JsonConvert.SerializeObject(model);
    }

    private async Task<IFiltersPageHelper> GetFiltersPageHelperAsync()
    {
        var queryString = Request.QueryString.Value;
        if (string.IsNullOrEmpty(queryString))
            await _filtersPageHelper.InitializeAsync(Url.ActionContext);
        else
            await _filtersPageHelper.InitializeAsync(Url.ActionContext, queryString);

        return _filtersPageHelper;
    }
}
