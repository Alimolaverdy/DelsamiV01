using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Framework.Components;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.PriceRangeFilterSlider;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;

[ViewComponent(Name = "NopAjaxFiltersPriceRangeFilter")]
public class PriceRangeFilterSliderComponent : Base7SpikesComponent
{
    private readonly IPermissionService _permissionService;
    private readonly IPriceCalculationServiceNopAjaxFilters _priceCalculationService;
    private readonly IPriceFormatter _priceFormatter;
    private readonly ISearchQueryStringHelper _searchQueryStringHelper;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly NopAjaxFiltersSettings _nopAjaxFiltersSettings;

    public PriceRangeFilterSliderComponent(
        IPermissionService permissionService,
        IPriceCalculationServiceNopAjaxFilters priceCalculationService,
        IPriceFormatter priceFormatter,
        ISearchQueryStringHelper searchQueryStringHelper,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IWorkContext workContext,
        NopAjaxFiltersSettings nopAjaxFiltersSettings)
    {
        _permissionService = permissionService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _searchQueryStringHelper = searchQueryStringHelper;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _nopAjaxFiltersSettings = nopAjaxFiltersSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync(int categoryId, int manufacturerId, int vendorId)
    {
        var queryParameters = _searchQueryStringHelper.GetQueryStringParameters(Request.QueryString.Value);
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices)
            || queryParameters.IsOnSearchPage)
        {
            return Content(string.Empty);
        }

        var model = await GetPriceRangeFilterInternalAsync(categoryId, manufacturerId, vendorId);
        if ((model.MaxPrice == 0m && model.MinPrice == 0m)
            || model.MinPrice == model.MaxPrice)
        {
            return Content(string.Empty);
        }

        return View("PriceRangeFilterSlider", model);
    }

    private async Task<PriceRangeFilterModel7Spikes> GetPriceRangeFilterInternalAsync(
        int categoryId,
        int manufacturerId,
        int vendorId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var currency = await _workContext.GetWorkingCurrencyAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var taxDisplayType = await _workContext.GetTaxDisplayTypeAsync();

        var cacheKeyArguments = new object[]
        {
            categoryId,
            manufacturerId,
            vendorId,
            customer.Id,
            currency.Id,
            language.Id,
            store.Id,
            taxDisplayType
        };

        var dtoCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_PRICE_RANGE_FILTER_DTO_KEY,
            cacheKeyArguments);

        var priceRangeDto = await _staticCacheManager.GetAsync(
            dtoCacheKey,
            () => _priceCalculationService.GetPriceRangeFilterDtoAsync(
                categoryId,
                manufacturerId,
                vendorId));

        var modelCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_PRICE_RANGE_FILTER_MODEL_KEY,
            cacheKeyArguments);

        var model = await _staticCacheManager.GetAsync<PriceRangeFilterModel7Spikes>(
            modelCacheKey,
            async () =>
            {
                var minPrice = priceRangeDto.MinPrice;
                var maxPrice = priceRangeDto.MaxPrice;

                if (minPrice == 0m && maxPrice == 0m)
                    return null;

                if (maxPrice - minPrice < 1m)
                {
                    minPrice = maxPrice;
                }
                else
                {
                    minPrice = Math.Floor(minPrice);
                    maxPrice = Math.Ceiling(maxPrice);
                }

                var currencySymbol = string.Empty;
                if (!string.IsNullOrEmpty(currency.DisplayLocale))
                {
                    currencySymbol = CultureInfo.GetCultureInfo(currency.DisplayLocale)
                        .NumberFormat.CurrencySymbol;
                }

                return new PriceRangeFilterModel7Spikes
                {
                    CategoryId = categoryId,
                    ManufacturerId = manufacturerId,
                    VendorId = vendorId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MinPriceFormatted = await GetFormattedPriceAsync(minPrice),
                    MaxPriceFormatted = await GetFormattedPriceAsync(maxPrice),
                    Formatting = currency.CustomFormatting,
                    CurrencySymbol = currencySymbol
                };
            });

        if (model == null || model.MinPrice == model.MaxPrice)
            return new PriceRangeFilterModel7Spikes();

        var selectedPriceRange = PriceRangeHelper.GetSelectedPriceRange() as PriceRangeModel;
        if (selectedPriceRange == null)
        {
            selectedPriceRange = new PriceRangeModel
            {
                From = Math.Floor(model.MinPrice),
                To = Math.Ceiling(model.MaxPrice)
            };
        }

        model.SelectedPriceRange = selectedPriceRange;
        return model;
    }

    private async Task<string> GetFormattedPriceAsync(decimal price)
    {
        var formattedPrice = await _priceFormatter.FormatPriceAsync(price, true, false);
        var trailingZeroesSeparator = _nopAjaxFiltersSettings.TrailingZeroesSeparator;

        if (string.IsNullOrEmpty(trailingZeroesSeparator))
            return formattedPrice;

        var pattern = $"{Regex.Escape(trailingZeroesSeparator)}0{{2,}}";
        return Regex.Replace(formattedPrice, pattern, string.Empty, RegexOptions.RightToLeft);
    }
}
