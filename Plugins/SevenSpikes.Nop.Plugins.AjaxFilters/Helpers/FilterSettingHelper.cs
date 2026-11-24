using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public static class FilterSettingHelper
{
    private static ISettingService _settingService;

    private static ISettingService SettingService =>
        _settingService ??= EngineContext.Current.Resolve<ISettingService>();

    public static async Task UpdateNopCommerceFilterSettings()
    {
        var stores = await EngineContext.Current.Resolve<IStoreService>().GetAllStoresAsync();
        await UpdateFilterSettingForStoreAsync(0);

        if (stores.Count <= 1
            || !await SettingService.GetSettingByKeyAsync<bool>(
                "SevenSpikesCommonSettings.LoadStoreSettingsOnLoad",
                true,
                0,
                false))
        {
            return;
        }

        foreach (var store in stores)
            await UpdateFilterSettingForStoreAsync(store.Id);
    }

    private static async Task UpdateFilterSettingForStoreAsync(int storeId)
    {
        var ajaxFiltersSettings = await SettingService.LoadSettingAsync<NopAjaxFiltersSettings>(storeId);
        if (!ajaxFiltersSettings.EnableAjaxFilters)
            return;

        var catalogSettings = await SettingService.LoadSettingAsync<CatalogSettings>(storeId);

        await DisableSettingIfEnabledAndOverriddenAsync(
            catalogSettings,
            catalogSettings.EnableManufacturerFiltering,
            settings => settings.EnableManufacturerFiltering,
            settings => settings.EnableManufacturerFiltering = false,
            storeId);

        await DisableSettingIfEnabledAndOverriddenAsync(
            catalogSettings,
            catalogSettings.EnableSpecificationAttributeFiltering,
            settings => settings.EnableSpecificationAttributeFiltering,
            settings => settings.EnableSpecificationAttributeFiltering = false,
            storeId);

        await DisableSettingIfEnabledAndOverriddenAsync(
            catalogSettings,
            catalogSettings.EnablePriceRangeFiltering,
            settings => settings.EnablePriceRangeFiltering,
            settings => settings.EnablePriceRangeFiltering = false,
            storeId);
    }

    private static async Task DisableSettingIfEnabledAndOverriddenAsync(
        CatalogSettings settings,
        bool isEnabled,
        Expression<Func<CatalogSettings, bool>> settingSelector,
        Action<CatalogSettings> disableSetting,
        int storeId)
    {
        if (!isEnabled
            || !await SettingService.SettingExistsAsync<CatalogSettings, bool>(
                settings,
                settingSelector,
                storeId))
        {
            return;
        }

        disableSetting(settings);
        await SettingService.SaveSettingAsync<CatalogSettings, bool>(
            settings,
            settingSelector,
            storeId,
            true);
    }
}
