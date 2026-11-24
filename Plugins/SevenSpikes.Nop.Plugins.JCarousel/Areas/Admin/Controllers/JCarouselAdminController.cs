using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using SevenSpikes.Nop.Framework;
using SevenSpikes.Nop.Framework.Areas.Admin.ControllerAttributes;
using SevenSpikes.Nop.Framework.Areas.Admin.Controllers;
using SevenSpikes.Nop.Framework.Areas.Admin.Helpers;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Services;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Controllers;

[ManagePluginsAdminAuthorize("SevenSpikes.Nop.Plugins.JCarousel", false)]
public class JCarouselAdminController : Base7SpikesAdminController
{
    private const string PluginSystemName = "SevenSpikes.Nop.Plugins.JCarousel";

    private readonly ILocalizationService _localizationService;
    private readonly IJCarouselService _jCarouselService;
    private readonly ILocalizedEntityService _localizedEntityService;
    private readonly ILanguageService _languageService;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;
    private readonly JCarouselEntitySettings _defaultJCarouselEntitySettings;

    public JCarouselAdminController(
        ILocalizationService localizationService,
        IJCarouselService jCarouselService,
        ILocalizedEntityService localizedEntityService,
        ILanguageService languageService,
        ISettingService settingService,
        WidgetSettings widgetSettings,
        JCarouselEntitySettings defaultJCarouselEntitySettings)
    {
        _localizationService = localizationService;
        _jCarouselService = jCarouselService;
        _localizedEntityService = localizedEntityService;
        _languageService = languageService;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
        _defaultJCarouselEntitySettings = defaultJCarouselEntitySettings;
    }

    public async Task<IActionResult> Settings()
    {
        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<JCarouselGeneralSettings>(storeScope);
        var model = settings.ToModel();

        model.ActiveStoreScopeConfiguration = storeScope;
        model.Enable = model.Enable && _widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName);
        model.IsTrialVersion = false;

        if (storeScope > 0)
        {
            var storeScopeSettings = new StoreScopeSettingsHelper<JCarouselGeneralSettings>(
                settings, storeScope, _settingService);

            model.Enable_OverrideForStore = await storeScopeSettings.SettingExistsAsync(
                setting => setting.Enable);
        }

        return View("Settings", model);
    }

    [HttpPost]
    public async Task<IActionResult> Settings(JCarouselGeneralSettingsModel model)
    {
        if (model.Enable && !_widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginSystemName);
            await _settingService.SaveSettingAsync(_widgetSettings, 0);
        }

        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var storeScopeSettings = new StoreScopeSettingsHelper<JCarouselGeneralSettings>(
            model.ToEntity(), storeScope, _settingService);

        await storeScopeSettings.SaveStoreSettingAsync(
            model.Enable_OverrideForStore,
            setting => setting.Enable);

        await _settingService.ClearCacheAsync();
        SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

        return RedirectToAction(nameof(Settings));
    }

    public async Task<IActionResult> Create()
    {
        var model = new JCarouselSettingsModel
        {
            AvailableDataSourceTypes = await GetAvailableDataSourceTypesAsync(),
            JCarouselEntitySettings = _defaultJCarouselEntitySettings.ToModel(),
            IsTrialVersion = false
        };

        await AddLocalesAsync<JCarouselSettingsLocalizedModel>(_languageService, model.Locales);
        model.AddCarouselToWidget.SupportedWidgetZones = new SelectList(
            await GetSupportedWidgetZonesAsync());

        return View("Create", model);
    }

    [HttpPost]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Create(JCarouselSettingsModel model, bool continueEditing)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableDataSourceTypes = await GetAvailableDataSourceTypesAsync();
            return View("Create", model);
        }

        var carousel = model.ToEntity();
        await _jCarouselService.InsertCarouselAsync(carousel);
        await UpdateLocalesAsync(carousel, model);

        SuccessNotification(await _localizationService.GetResourceAsync(
            "SevenSpikes.JCarousel.Admin.Created"));

        if (!continueEditing)
            return RedirectToAction(nameof(List));

        SaveSelectedTabName(string.Empty, true);
        return RedirectToAction(nameof(Edit), new { id = carousel.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var carousel = await _jCarouselService.GetCarouselByIdAsync(id);
        if (carousel == null)
            return NotFound();

        var model = carousel.ToModel();
        model.AvailableDataSourceTypes = await GetAvailableDataSourceTypesAsync();
        model.AddCarouselToWidget.SupportedWidgetZones = new SelectList(
            await GetSupportedWidgetZonesAsync());

        await AddLocalesAsync<JCarouselSettingsLocalizedModel>(_languageService, model.Locales);
        foreach (var locale in model.Locales)
        {
            locale.Title = await LocalizationService.GetLocalizedAsync<Carousel, string>(
                carousel,
                entity => entity.Title,
                locale.LanguageId,
                returnDefaultValue: false,
                ensureTwoPublishedLanguages: false);
        }

        model.ProductMapping = new JCarouselProductMappingModel
        {
            JCarouselId = carousel.Id
        };

        await PrepareStoresMappingModelAsync(model.StoreMappingModel, carousel, excludeProperties: false);
        model.IsTrialVersion = false;

        return View("Edit", model);
    }

    [HttpPost]
    [ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Edit(JCarouselSettingsModel model, bool continueEditing)
    {
        var carousel = await _jCarouselService.GetCarouselByIdAsync(model.Id);
        if (carousel == null)
            throw new ArgumentException("No carousel found with the specified id", nameof(model));

        if (!ModelState.IsValid)
        {
            await PrepareStoresMappingModelAsync(model.StoreMappingModel, carousel, excludeProperties: false);
            model.AvailableDataSourceTypes = await GetAvailableDataSourceTypesAsync();
            return View("Edit", model);
        }

        carousel = model.ToEntity(carousel);
        await SaveStoreMappingsAsync(carousel, model.StoreMappingModel);
        await _jCarouselService.UpdateCarouselAsync(carousel);
        await UpdateLocalesAsync(carousel, model);

        SuccessNotification(await _localizationService.GetResourceAsync(
            "SevenSpikes.JCarousel.Admin.Edited"));

        if (!continueEditing)
            return RedirectToAction(nameof(List));

        SaveSelectedTabName(string.Empty, true);
        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    public async Task<IActionResult> List()
    {
        var model = await PrepareListModelAsync();
        return View("List", model);
    }

    [HttpPost]
    public async Task<IActionResult> List(JCarouselSettingsSearchModel searchModel)
    {
        var models = await PrepareListModelAsync();
        var pagedList = new PagedList<JCarouselSettingsModel>(
            models,
            searchModel.Page - 1,
            searchModel.PageSize,
            null);

        var gridModel = new JCarouselSettingsListModel();
        gridModel.PrepareToGrid(searchModel, pagedList, () => pagedList);

        return Json(gridModel);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCarousel(int id)
    {
        var carousel = await _jCarouselService.GetCarouselByIdAsync(id);
        if (carousel == null)
            return NotFound();

        await _jCarouselService.DeleteCarouselAsync(carousel);
        return EmptyJson;
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var carousel = await _jCarouselService.GetCarouselByIdAsync(id);
        if (carousel == null)
            return NotFound();

        await _jCarouselService.DeleteCarouselAsync(carousel);
        return RedirectToAction(nameof(List));
    }

    private async Task<List<JCarouselSettingsModel>> PrepareListModelAsync()
    {
        var carousels = await _jCarouselService.GetAllCarouselsAsync();
        var models = new List<JCarouselSettingsModel>(carousels.Count);

        foreach (var carousel in carousels)
        {
            var model = carousel.ToModel();
            model.DataSourceType = await JCarouselDataSourceTypes.GetLocalizedAsync(
                model.DataSourceType,
                _localizationService);

            models.Add(model);
        }

        return models;
    }

    [NonAction]
    private async Task UpdateLocalesAsync(Carousel carousel, JCarouselSettingsModel model)
    {
        foreach (var locale in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(
                carousel,
                entity => entity.Title,
                locale.Title,
                locale.LanguageId);
        }
    }

    private async Task<IList<SelectListItem>> GetAvailableDataSourceTypesAsync()
    {
        string[] dataSourceTypes =
        {
            "None",
            "HomePageFeaturedProducts",
            "BestSellersProducts",
            "BestSellersProductsByQuantity",
            "RecentlyAddedProducts",
            "MarkedAsNewProducts",
            "RecentlyViewedProducts",
            "RelatedProducts",
            "AlsoPurchasedProducts",
            "ProductsFromTheSameCategory",
            "ProductsFromTheSameManufacturer",
            "CategoryFeaturedProducts",
            "ManufacturerFeaturedProducts",
            "CrossSellProducts",
            "Manufacturers",
            "Vendors",
            "ProductsFromTheSameVendor",
            "HomePageCategories",
            "SubCategories",
            "SiblingCategories"
        };

        var result = new List<SelectListItem>(dataSourceTypes.Length);
        foreach (var dataSourceType in dataSourceTypes)
        {
            result.Add(new SelectListItem
            {
                Text = await JCarouselDataSourceTypes.GetLocalizedAsync(
                    dataSourceType,
                    _localizationService),
                Value = dataSourceType
            });
        }

        return result;
    }

    private Task<IEnumerable<string>> GetSupportedWidgetZonesAsync()
    {
        return EngineContext.Current
            .Resolve<IInstallHelper>()
            .GetSupportedWidgetZonesAsync(PluginSystemName);
    }
}
