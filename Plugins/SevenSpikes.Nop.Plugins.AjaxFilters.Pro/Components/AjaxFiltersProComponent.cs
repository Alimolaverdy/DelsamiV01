using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using SevenSpikes.Nop.Core;
using SevenSpikes.Nop.Core.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Enums;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Settings;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Models;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Components;

[ViewComponent(Name = "NopAjaxFiltersPro")]
public class AjaxFiltersProComponent : ViewComponent
{
    private const string AjaxFiltersPluginSystemName = "SevenSpikes.Nop.Plugins.AjaxFilters";

    private readonly ICategoryService _categoryService;
    private readonly IConvertToDictionaryHelper _convertToDictionaryHelper;
    private readonly IManufacturerService _manufacturerService;
    private readonly ISpecificationAttributeService _specificationAttributeService;
    private readonly NopAjaxFiltersSettings _ajaxFiltersSettings;
    private readonly NopAjaxFiltersProSettings _ajaxFiltersProSettings;
    private readonly WidgetSettings _widgetSettings;
    private readonly ILocalizationService _localizationService;

    public AjaxFiltersProComponent(
        ICategoryService categoryService,
        IConvertToDictionaryHelper convertToDictionaryHelper,
        IManufacturerService manufacturerService,
        ISpecificationAttributeService specificationAttributeService,
        NopAjaxFiltersSettings ajaxFiltersSettings,
        NopAjaxFiltersProSettings ajaxFiltersProSettings,
        WidgetSettings widgetSettings,
        ILocalizationService localizationService)
    {
        _categoryService = categoryService;
        _convertToDictionaryHelper = convertToDictionaryHelper;
        _manufacturerService = manufacturerService;
        _specificationAttributeService = specificationAttributeService;
        _ajaxFiltersSettings = ajaxFiltersSettings;
        _ajaxFiltersProSettings = ajaxFiltersProSettings;
        _widgetSettings = widgetSettings;
        _localizationService = localizationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone)
    {
        if (_ajaxFiltersProSettings.WidgetZone != widgetZone
            || !_ajaxFiltersProSettings.EnableAjaxFiltersPro
            || !_ajaxFiltersSettings.EnableAjaxFilters
            || !_widgetSettings.ActiveWidgetSystemNames.Contains(AjaxFiltersPluginSystemName))
        {
            return Content(string.Empty);
        }

        var excludedCategoryIds = (_ajaxFiltersSettings.CategoriesWithoutFilters ?? string.Empty)
            .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .ToHashSet();

        switch (_ajaxFiltersProSettings.UIMode)
        {
            case NopAjaxFiltersProUIMode.CategoryAndManufacturer:
            case NopAjaxFiltersProUIMode.CategorySubcategoryAndManufacturer:
                {
                    var categories = await _categoryService.GetAllCategoriesAsync(0, false);
                    var hasAvailableCategories = categories.Any(category =>
                        !excludedCategoryIds.Contains(category.Id.ToString()));
                    var manufacturers = await _manufacturerService.GetAllManufacturersAsync(
                        "",
                        0,
                        0,
                        int.MaxValue,
                        false,
                        null);

                    if (!hasAvailableCategories && !manufacturers.Any())
                        return Content(string.Empty);

                    break;
                }

            case NopAjaxFiltersProUIMode.CategoryOnly:
            case NopAjaxFiltersProUIMode.CategoryAndSubcategory:
                {
                    var categories = await _categoryService.GetAllCategoriesAsync(0, false);
                    if (!categories.Any(category => !excludedCategoryIds.Contains(category.Id.ToString())))
                        return Content(string.Empty);

                    break;
                }

            case NopAjaxFiltersProUIMode.ManufacturerOnly:
                {
                    var manufacturers = await _manufacturerService.GetAllManufacturersAsync(
                        "",
                        0,
                        0,
                        int.MaxValue,
                        false,
                        null);

                    if (!manufacturers.Any())
                        return Content(string.Empty);

                    break;
                }
        }

        return await GetSearchWidgetViewAsync();
    }

    private async Task<IViewComponentResult> GetSearchWidgetViewAsync()
    {
        var selectedSpecificationIds = _convertToDictionaryHelper
            .CreateDictionaryFromSemicolonSeparatedPairsSortedByValue<int, int>(
                _ajaxFiltersProSettings.ActiveSpecificationAttributes)
            .Keys
            .ToArray();

        var specifications = BaseEntityExtensions.SortAs<SpecificationAttribute>(
          await _specificationAttributeService.GetSpecificationAttributeByIdsAsync(selectedSpecificationIds),
            selectedSpecificationIds);

        var specificationModels = new List<BaseNopAjaxFiltersProModel>(specifications.Count);
        foreach (var specification in specifications)
        {
            specificationModels.Add(new BaseNopAjaxFiltersProModel
            {
                Id = specification.Id,
                Name = await _localizationService.GetLocalizedAsync<SpecificationAttribute, string>(
                    specification,
                    item => item.Name,
                    null,
                    true,
                    true)
            });
        }

        var model = new NopAjaxFiltersProPublicModel
        {
            Specifications = specificationModels,
            UIMode = _ajaxFiltersProSettings.UIMode
        };

        return View("NopFiltersPro", model);
    }
}
