using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Kendoui;
using SevenSpikes.Nop.Core;
using SevenSpikes.Nop.Core.Helpers;
using SevenSpikes.Nop.Framework;
using SevenSpikes.Nop.Framework.Areas.Admin.ControllerAttributes;
using SevenSpikes.Nop.Framework.Areas.Admin.Controllers;
using SevenSpikes.Nop.Framework.Areas.Admin.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Enums;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Settings;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Extensions;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Controllers;

[ManagePluginsAdminAuthorize("SevenSpikes.Nop.Plugins.AjaxFilters.Pro", false)]
public class AjaxFiltersProAdminController : Base7SpikesAdminController
{
    private const string PluginSystemName = "SevenSpikes.Nop.Plugins.AjaxFilters.Pro";

    private readonly ICustomerActivityService _customerActivityService;
    private readonly IConvertToDictionaryHelper _convertToDictionaryHelper;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly ISpecificationAttributeService _specificationAttributeService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly NopAjaxFiltersProSettings _settings;
    private readonly WidgetSettings _widgetSettings;

    public AjaxFiltersProAdminController(
        ICustomerActivityService customerActivityService,
        IConvertToDictionaryHelper convertToDictionaryHelper,
        ILocalizationService localizationService,
        ISettingService settingService,
        ISpecificationAttributeService specificationAttributeService,
        IStaticCacheManager staticCacheManager,
        NopAjaxFiltersProSettings settings,
        WidgetSettings widgetSettings)
    {
        _customerActivityService = customerActivityService;
        _convertToDictionaryHelper = convertToDictionaryHelper;
        _localizationService = localizationService;
        _settingService = settingService;
        _specificationAttributeService = specificationAttributeService;
        _staticCacheManager = staticCacheManager;
        _settings = settings;
        _widgetSettings = widgetSettings;
    }

    public async Task<ActionResult> Settings()
    {
        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<NopAjaxFiltersProSettings>(storeScope);
        var model = settings.ToModel();

        await InitializeModelAsync(model, storeScope);
        model.ActiveStoreScopeConfiguration = storeScope;

        if (storeScope > 0)
        {
            var storeSettings = new StoreScopeSettingsHelper<NopAjaxFiltersProSettings>(
                settings,
                storeScope,
                _settingService);

            model.EnableAjaxFiltersPro_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableAjaxFiltersPro);
            model.WidgetZone_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.WidgetZone);
            model.UIMode_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.UIMode);
            model.ShowTopLevelCategoriesOnly_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.ShowTopLevelCategoriesOnly);
        }

        return View(nameof(Settings), model);
    }

    [HttpPost]
    public async Task<ActionResult> Settings(NopAjaxFiltersProSettingsModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Settings));

        if (model.EnableAjaxFiltersPro && !_widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginSystemName);
            await _settingService.SaveSettingAsync<WidgetSettings>(_widgetSettings, 0);
        }

        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var storeSettings = new StoreScopeSettingsHelper<NopAjaxFiltersProSettings>(
            model.ToEntity(),
            storeScope,
            _settingService);

        await storeSettings.SaveStoreSettingAsync(
            model.EnableAjaxFiltersPro_OverrideForStore,
            setting => setting.EnableAjaxFiltersPro);
        await storeSettings.SaveStoreSettingAsync(
            model.WidgetZone_OverrideForStore,
            setting => setting.WidgetZone);
        await storeSettings.SaveStoreSettingAsync(
            model.UIMode_OverrideForStore,
            setting => setting.UIMode);
        await storeSettings.SaveStoreSettingAsync(
            model.ShowTopLevelCategoriesOnly_OverrideForStore,
            setting => setting.ShowTopLevelCategoriesOnly);

        await _settingService.ClearCacheAsync();
        await _staticCacheManager.RemoveByPrefixAsync("nop.pres.nop.ajax.filters");
        await _customerActivityService.InsertActivityAsync(
            "EditNopAjaxFiltersProSettings",
            "Edit Nop Ajax Filters Pro Settings");

        SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));
        return RedirectToAction(nameof(Settings));
    }

    private async Task InitializeModelAsync(NopAjaxFiltersProSettingsModel model, int storeScope)
    {
        var supportedWidgetZones = await EngineContext.Current
            .Resolve<IInstallHelper>()
            .GetSupportedWidgetZonesAsync(PluginSystemName, storeScope);

        model.SupportedWidgetZones = new SelectList(supportedWidgetZones);
        model.AvailableUiModes = await model.UIMode.ToSelectListAsync<NopAjaxFiltersProUIMode>();
        model.EnableAjaxFiltersPro = model.EnableAjaxFiltersPro
            && _widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName);
        model.IsTrialVersion = false;
    }

    [HttpPost]
    public ActionResult GridList(DataSourceRequest command)
    {
        var activeSpecifications = _settings.ActiveSpecificationAttributes;
        IList<SpecificationAttribute> specifications = new List<SpecificationAttribute>();
        Dictionary<int, int> displayOrders = new Dictionary<int, int>();

        if (activeSpecifications.Count > 0)
        {
            displayOrders = (Dictionary<int, int>)_convertToDictionaryHelper
                .CreateDictionaryFromSemicolonSeparatedPairsSortedByValue<int, int>(activeSpecifications);

            var specificationIds = displayOrders.Keys.ToArray();
            specifications = BaseEntityExtensions.SortAs<SpecificationAttribute>(
                _specificationAttributeService.GetSpecificationAttributeByIds(specificationIds),
                specificationIds);
        }

        var data = specifications.Select(specification => new
        {
            specification.Id,
            specification.Name,
            DisplayOrder = displayOrders[specification.Id]
        });

        return Json(new DataSourceResult
        {
            Data = data,
            Total = specifications.Count
        });
    }

    [HttpPost]
    public async Task<ActionResult> Update(DataSourceRequest command, int id, int displayOrder)
    {
        if (id == 0)
            return EmptyJson;

        var activeSpecifications = _settings.ActiveSpecificationAttributes;
        var displayOrders = _convertToDictionaryHelper
            .CreateDictionaryFromSemicolonSeparatedPairsSortedByValue<int, int>(activeSpecifications);

        if (displayOrders.TryGetValue(id, out var currentDisplayOrder))
        {
            activeSpecifications.Remove($"{id}:{currentDisplayOrder}");
            activeSpecifications.Add($"{id}:{displayOrder}");
            await _settingService.SaveSettingAsync(_settings);
        }

        return GridList(command);
    }

    [HttpPost]
    public async Task<ActionResult> Delete(DataSourceRequest command, int id)
    {
        if (id == 0)
            return EmptyJson;

        var activeSpecifications = _settings.ActiveSpecificationAttributes;
        var displayOrders = _convertToDictionaryHelper
            .CreateDictionaryFromSemicolonSeparatedPairsSortedByValue<int, int>(activeSpecifications);

        if (displayOrders.TryGetValue(id, out var currentDisplayOrder))
        {
            activeSpecifications.Remove($"{id}:{currentDisplayOrder}");
            await _settingService.SaveSettingAsync(_settings);
        }

        return GridList(command);
    }

    [HttpPost]
    public async Task<ActionResult> Create(DataSourceRequest command, string name, int displayOrder)
    {
        if (!string.IsNullOrEmpty(name))
        {
            var specification = _specificationAttributeService.GetSpecificationAttributeByName(name);
            var activeSpecifications = _settings.ActiveSpecificationAttributes;
            var displayOrders = _convertToDictionaryHelper
                .CreateDictionaryFromSemicolonSeparatedPairsSortedByValue<int, int>(activeSpecifications);

            if (specification != null && !displayOrders.ContainsKey(specification.Id))
            {
                activeSpecifications.Add($"{specification.Id}:{displayOrder}");
                await _settingService.SaveSettingAsync(_settings);
            }
        }

        return GridList(command);
    }

    [HttpGet]
    public async Task<ActionResult> GetSpecifications()
    {
        var activeSpecifications = _settings.ActiveSpecificationAttributes;
        var activeSpecificationIds = _convertToDictionaryHelper
            .CreateDictionaryFromSemicolonSeparatedPairsSortedByValue<int, int>(activeSpecifications)
            .Keys;

        var availableSpecifications = await _specificationAttributeService.GetSpecificationAttributesAsync();
        var model = availableSpecifications
            .Where(specification => !activeSpecificationIds.Contains(specification.Id))
            .Select(specification => new SelectListItem
            {
                Value = specification.Id.ToString(),
                Text = specification.Name
            })
            .ToList();

        return Json(model);
    }
}
