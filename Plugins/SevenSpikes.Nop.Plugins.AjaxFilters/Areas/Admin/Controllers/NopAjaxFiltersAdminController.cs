using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using SevenSpikes.Nop.Core.Helpers;
using SevenSpikes.Nop.Framework;
using SevenSpikes.Nop.Framework.Areas.Admin.ControllerAttributes;
using SevenSpikes.Nop.Framework.Areas.Admin.Controllers;
using SevenSpikes.Nop.Framework.Areas.Admin.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.AjaxFilters.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain.Enums;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Areas.Admin.Controllers;

[ManagePluginsAdminAuthorize("SevenSpikes.Nop.Plugins.AjaxFilters", false)]
public class NopAjaxFiltersAdminController : Base7SpikesAdminController
{
    private readonly IConvertToDictionaryHelper _convertToDictionaryHelper;

    private readonly ICustomerActivityService _customerActivityService;

    private readonly ILocalizationService _localizationService;

    private readonly IProductAttributeService _productAttributeService;

    private readonly ISettingService _settingService;

    private readonly ISpecificationAttributeService _specificationAttributeService;

    private readonly IStaticCacheManager _staticCacheManager;

    private readonly WidgetSettings _widgetSettings;

    private readonly NopAjaxFiltersSettings _nopAjaxFiltersSettings;

    public NopAjaxFiltersAdminController(IConvertToDictionaryHelper convertToDictionaryHelper, ICustomerActivityService customerActivityService, ILocalizationService localizationService, IProductAttributeService productAttributeService, ISettingService settingService, ISpecificationAttributeService specificationAttributeService, IStaticCacheManager staticCacheManager, WidgetSettings widgetSettings, NopAjaxFiltersSettings nopAjaxFiltersSettings)
    {
        _convertToDictionaryHelper = convertToDictionaryHelper;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _productAttributeService = productAttributeService;
        _settingService = settingService;
        _specificationAttributeService = specificationAttributeService;
        _staticCacheManager = staticCacheManager;
        _widgetSettings = widgetSettings;
        _nopAjaxFiltersSettings = nopAjaxFiltersSettings;
    }

    public async Task<ActionResult> Settings()
    {
        int storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        NopAjaxFiltersSettings nopAjaxFiltersSettings = await this._settingService.LoadSettingAsync<NopAjaxFiltersSettings>(storeScope);
        NopAjaxFiltersSettingsModel model = nopAjaxFiltersSettings.ToModel();
        await this.InitializeModelAsync(model, storeScope);
        model.ActiveStoreScopeConfiguration = storeScope;
        if (storeScope > 0)
        {
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> storeScopeSettings = new StoreScopeSettingsHelper<NopAjaxFiltersSettings>(nopAjaxFiltersSettings, storeScope, this._settingService);
            NopAjaxFiltersSettingsModel filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper1 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector1 = (NopAjaxFiltersSettings x) => x.EnableAjaxFilters;
            NopAjaxFiltersSettingsModel nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableAjaxFilters_OverrideForStore = await scopeSettingsHelper1.SettingExistsAsync<bool>(keySelector1);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper12 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, FiltersUIMode>> keySelector12 = (NopAjaxFiltersSettings x) => x.FiltersUIMode;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.FiltersUIMode_OverrideForStore = await scopeSettingsHelper12.SettingExistsAsync<FiltersUIMode>(keySelector12);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper23 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector23 = (NopAjaxFiltersSettings x) => x.EnableInfiniteScroll;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableInfiniteScroll_OverrideForStore = await scopeSettingsHelper23.SettingExistsAsync<bool>(keySelector23);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper34 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector34 = (NopAjaxFiltersSettings x) => x.ScrollToElementOnThePageAfterFiltration;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ScrollToElementOnThePageAfterFiltration_OverrideForStore = await scopeSettingsHelper34.SettingExistsAsync<bool>(keySelector34);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper37 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector37 = (NopAjaxFiltersSettings x) => x.ScrollToElementOnThePageAfterFiltrationMobile;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ScrollToElementOnThePageAfterFiltrationMobile_OverrideForStore = await scopeSettingsHelper37.SettingExistsAsync<bool>(keySelector37);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper38 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector38 = (NopAjaxFiltersSettings x) => x.ElementToScrollAfterFiltrationSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ElementToScrollAfterFiltrationSelector_OverrideForStore = await scopeSettingsHelper38.SettingExistsAsync<string>(keySelector38);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper39 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector39 = (NopAjaxFiltersSettings x) => x.WidgetZone;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.WidgetZone_OverrideForStore = await scopeSettingsHelper39.SettingExistsAsync<string>(keySelector39);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper40 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector40 = (NopAjaxFiltersSettings x) => x.EnablePriceRangeFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnablePriceRangeFilter_OverrideForStore = await scopeSettingsHelper40.SettingExistsAsync<bool>(keySelector40);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper41 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector41 = (NopAjaxFiltersSettings x) => x.ClosePriceRangeFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ClosePriceRangeFilterBox_OverrideForStore = await scopeSettingsHelper41.SettingExistsAsync<bool>(keySelector41);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper2 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector2 = (NopAjaxFiltersSettings x) => x.EnableSpecificationsFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableSpecificationsFilter_OverrideForStore = await scopeSettingsHelper2.SettingExistsAsync<bool>(keySelector2);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper3 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector3 = (NopAjaxFiltersSettings x) => x.CloseSpecificationsFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseSpecificationsFilterBox_OverrideForStore = await scopeSettingsHelper3.SettingExistsAsync<bool>(keySelector3);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper4 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, int>> keySelector4 = (NopAjaxFiltersSettings x) => x.NumberOfSpecificationFilters;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.NumberOfSpecificationFilters_OverrideForStore = await scopeSettingsHelper4.SettingExistsAsync<int>(keySelector4);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper5 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector5 = (NopAjaxFiltersSettings x) => x.EnableAttributesFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableAttributesFilter_OverrideForStore = await scopeSettingsHelper5.SettingExistsAsync<bool>(keySelector5);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper6 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector6 = (NopAjaxFiltersSettings x) => x.CloseAttributesFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseAttributesFilterBox_OverrideForStore = await scopeSettingsHelper6.SettingExistsAsync<bool>(keySelector6);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper7 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, int>> keySelector7 = (NopAjaxFiltersSettings x) => x.NumberOfAttributeFilters;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.NumberOfAttributeFilters_OverrideForStore = await scopeSettingsHelper7.SettingExistsAsync<int>(keySelector7);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper8 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector8 = (NopAjaxFiltersSettings x) => x.EnableManufacturersFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableManufacturersFilter_OverrideForStore = await scopeSettingsHelper8.SettingExistsAsync<bool>(keySelector8);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper9 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector9 = (NopAjaxFiltersSettings x) => x.CloseManufacturersFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseManufacturersFilterBox_OverrideForStore = await scopeSettingsHelper9.SettingExistsAsync<bool>(keySelector9);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper10 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector10 = (NopAjaxFiltersSettings x) => x.EnableOnSaleFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableOnSaleFilter_OverrideForStore = await scopeSettingsHelper10.SettingExistsAsync<bool>(keySelector10);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper11 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector11 = (NopAjaxFiltersSettings x) => x.CloseOnSaleFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseOnSaleFilterBox_OverrideForStore = await scopeSettingsHelper11.SettingExistsAsync<bool>(keySelector11);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper13 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector13 = (NopAjaxFiltersSettings x) => x.EnableVendorsFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableVendorsFilter_OverrideForStore = await scopeSettingsHelper13.SettingExistsAsync<bool>(keySelector13);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper14 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector14 = (NopAjaxFiltersSettings x) => x.CloseVendorsFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseVendorsFilterBox_OverrideForStore = await scopeSettingsHelper14.SettingExistsAsync<bool>(keySelector14);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper15 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector15 = (NopAjaxFiltersSettings x) => x.EnableInStockFilter;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.EnableInStockFilter_OverrideForStore = await scopeSettingsHelper15.SettingExistsAsync<bool>(keySelector15);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper16 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector16 = (NopAjaxFiltersSettings x) => x.CloseInStockFilterBox;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseInStockFilterBox_OverrideForStore = await scopeSettingsHelper16.SettingExistsAsync<bool>(keySelector16);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper17 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector17 = (NopAjaxFiltersSettings x) => x.ProductsListPanelSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ProductsListPanelSelector_OverrideForStore = await scopeSettingsHelper17.SettingExistsAsync<string>(keySelector17);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper18 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector18 = (NopAjaxFiltersSettings x) => x.CategoriesWithoutFilters;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CategoriesWithoutFilters_OverrideForStore = await scopeSettingsHelper18.SettingExistsAsync<string>(keySelector18);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper19 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector19 = (NopAjaxFiltersSettings x) => x.ProductsGridPanelSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ProductsGridPanelSelector_OverrideForStore = await scopeSettingsHelper19.SettingExistsAsync<string>(keySelector19);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper20 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector20 = (NopAjaxFiltersSettings x) => x.PagerPanelSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.PagerPanelSelector_OverrideForStore = await scopeSettingsHelper20.SettingExistsAsync<string>(keySelector20);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper21 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector21 = (NopAjaxFiltersSettings x) => x.PagerPanelIntegrationSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.PagerPanelIntegrationSelector_OverrideForStore = await scopeSettingsHelper21.SettingExistsAsync<string>(keySelector21);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper22 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector22 = (NopAjaxFiltersSettings x) => x.SortOptionsDropDownSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.SortOptionsDropDownSelector_OverrideForStore = await scopeSettingsHelper22.SettingExistsAsync<string>(keySelector22);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper24 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector24 = (NopAjaxFiltersSettings x) => x.ViewOptionsDropDownSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ViewOptionsDropDownSelector_OverrideForStore = await scopeSettingsHelper24.SettingExistsAsync<string>(keySelector24);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper25 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector25 = (NopAjaxFiltersSettings x) => x.ProductPageSizeDropDownSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ProductPageSizeDropDownSelector_OverrideForStore = await scopeSettingsHelper25.SettingExistsAsync<string>(keySelector25);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper26 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector26 = (NopAjaxFiltersSettings x) => x.ShowFiltersOnCategoryPage;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowFiltersOnCategoryPage_OverrideForStore = await scopeSettingsHelper26.SettingExistsAsync<bool>(keySelector26);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper27 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector27 = (NopAjaxFiltersSettings x) => x.ShowFiltersOnManufacturerPage;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowFiltersOnManufacturerPage_OverrideForStore = await scopeSettingsHelper27.SettingExistsAsync<bool>(keySelector27);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper28 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector28 = (NopAjaxFiltersSettings x) => x.ShowFiltersOnVendorPage;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowFiltersOnVendorPage_OverrideForStore = await scopeSettingsHelper28.SettingExistsAsync<bool>(keySelector28);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper29 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector29 = (NopAjaxFiltersSettings x) => x.ShowFiltersOnSearchPage;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowFiltersOnSearchPage_OverrideForStore = await scopeSettingsHelper29.SettingExistsAsync<bool>(keySelector29);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper30 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector30 = (NopAjaxFiltersSettings x) => x.ShowSelectedFiltersPanel;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowSelectedFiltersPanel_OverrideForStore = await scopeSettingsHelper30.SettingExistsAsync<bool>(keySelector30);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper31 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector31 = (NopAjaxFiltersSettings x) => x.ShowNumberOfReturnedProducts;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowNumberOfReturnedProducts_OverrideForStore = await scopeSettingsHelper31.SettingExistsAsync<bool>(keySelector31);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper32 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector32 = (NopAjaxFiltersSettings x) => x.ShowNumberOfReturnedProductsSelector;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.ShowNumberOfReturnedProductsSelector_OverrideForStore = await scopeSettingsHelper32.SettingExistsAsync<string>(keySelector32);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper33 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, string>> keySelector33 = (NopAjaxFiltersSettings x) => x.TrailingZeroesSeparator;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.TrailingZeroesSeparator_OverrideForStore = await scopeSettingsHelper33.SettingExistsAsync<string>(keySelector33);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper35 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector35 = (NopAjaxFiltersSettings x) => x.SearchInProductTags;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.SearchInProductTags_OverrideForStore = await scopeSettingsHelper35.SettingExistsAsync<bool>(keySelector35);
            filtersSettingsModel = model;
            StoreScopeSettingsHelper<NopAjaxFiltersSettings> scopeSettingsHelper36 = storeScopeSettings;
            Expression<Func<NopAjaxFiltersSettings, bool>> keySelector36 = (NopAjaxFiltersSettings x) => x.CloseFiltersPanelAfterFiltrationInMobile;
            nopAjaxFiltersSettingsModel = filtersSettingsModel;
            nopAjaxFiltersSettingsModel.CloseFiltersPanelAfterFiltrationInMobile_OverrideForStore = await scopeSettingsHelper36.SettingExistsAsync<bool>(keySelector36);
        }
        return View("Settings", model);
    }

    [HttpPost]
    public async Task<ActionResult> SettingsAsync(NopAjaxFiltersSettingsModel nopAjaxFiltersSettingsModel)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Settings");
        }
        if (nopAjaxFiltersSettingsModel.EnableAjaxFilters && !this._widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.AjaxFilters"))
        {
            this._widgetSettings.ActiveWidgetSystemNames.Add("SevenSpikes.Nop.Plugins.AjaxFilters");
            await this._settingService.SaveSettingAsync<WidgetSettings>(this._widgetSettings, 0);
        }
        int configurationAsync = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        StoreScopeSettingsHelper<NopAjaxFiltersSettings> storeScopeSettings = new StoreScopeSettingsHelper<NopAjaxFiltersSettings>(nopAjaxFiltersSettingsModel.ToEntity(), configurationAsync, this._settingService);
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableAjaxFilters_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableAjaxFilters));
        await storeScopeSettings.SaveStoreSettingAsync<FiltersUIMode>((nopAjaxFiltersSettingsModel.FiltersUIMode_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, FiltersUIMode>>)((NopAjaxFiltersSettings x) => x.FiltersUIMode));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableInfiniteScroll_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableInfiniteScroll));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ScrollToElementOnThePageAfterFiltration_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ScrollToElementOnThePageAfterFiltration));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ScrollToElementOnThePageAfterFiltrationMobile_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ScrollToElementOnThePageAfterFiltrationMobile));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.ElementToScrollAfterFiltrationSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.ElementToScrollAfterFiltrationSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.WidgetZone_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.WidgetZone));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnablePriceRangeFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnablePriceRangeFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ClosePriceRangeFilterBox_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ClosePriceRangeFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableSpecificationsFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableSpecificationsFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseSpecificationsFilterBox_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseSpecificationsFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<int>((nopAjaxFiltersSettingsModel.NumberOfSpecificationFilters_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, int>>)((NopAjaxFiltersSettings x) => x.NumberOfSpecificationFilters));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableAttributesFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableAttributesFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseAttributesFilterBox_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseAttributesFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<int>((nopAjaxFiltersSettingsModel.NumberOfAttributeFilters_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, int>>)((NopAjaxFiltersSettings x) => x.NumberOfAttributeFilters));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableManufacturersFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableManufacturersFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseManufacturersFilterBox_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseManufacturersFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableOnSaleFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableOnSaleFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseOnSaleFilterBox_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseOnSaleFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableVendorsFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableVendorsFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseVendorsFilterBox_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseVendorsFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.EnableInStockFilter_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.EnableInStockFilter));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseInStockFilterBox ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseInStockFilterBox));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.ProductsListPanelSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.ProductsListPanelSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.CategoriesWithoutFilters_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.CategoriesWithoutFilters));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.ProductsGridPanelSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.ProductsGridPanelSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.PagerPanelSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.PagerPanelSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.PagerPanelIntegrationSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.PagerPanelIntegrationSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.SortOptionsDropDownSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.SortOptionsDropDownSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.ViewOptionsDropDownSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.ViewOptionsDropDownSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.ProductPageSizeDropDownSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.ProductPageSizeDropDownSelector));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ShowFiltersOnManufacturerPage_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ShowFiltersOnManufacturerPage));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ShowFiltersOnVendorPage_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ShowFiltersOnVendorPage));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ShowFiltersOnCategoryPage_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ShowFiltersOnCategoryPage));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ShowFiltersOnSearchPage_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ShowFiltersOnSearchPage));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ShowSelectedFiltersPanel_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ShowSelectedFiltersPanel));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.ShowNumberOfReturnedProducts_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.ShowNumberOfReturnedProducts));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.ShowNumberOfReturnedProductsSelector_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.ShowNumberOfReturnedProductsSelector));
        await storeScopeSettings.SaveStoreSettingAsync<string>((nopAjaxFiltersSettingsModel.TrailingZeroesSeparator_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, string>>)((NopAjaxFiltersSettings x) => x.TrailingZeroesSeparator));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.SearchInProductTags_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.SearchInProductTags));
        await storeScopeSettings.SaveStoreSettingAsync<bool>((nopAjaxFiltersSettingsModel.CloseFiltersPanelAfterFiltrationInMobile_OverrideForStore ? 1 : 0) != 0, (Expression<Func<NopAjaxFiltersSettings, bool>>)((NopAjaxFiltersSettings x) => x.CloseFiltersPanelAfterFiltrationInMobile));
        await this._settingService.ClearCacheAsync();
        await this._staticCacheManager.RemoveByPrefixAsync("nop.pres.nop.ajax.filters", Array.Empty<object>());
        await this._customerActivityService.InsertActivityAsync("EditNopAjaxFiltersSettings", "Edit Nop Ajax Filters Settings", (BaseEntity)null);
        await FilterSettingHelper.UpdateNopCommerceFilterSettings();
        SuccessNotification(await this._localizationService.GetResourceAsync("Admin.Configuration.Updated"));
        SaveSelectedTabName("", true);
        return RedirectToAction("Settings");
    }

    private async Task InitializeModelAsync(NopAjaxFiltersSettingsModel model, int storeScope)
    {
        NopAjaxFiltersSettingsModel nopAjaxFiltersSettingsModel = model;
        nopAjaxFiltersSettingsModel.SupportedWidgetZones = new SelectList(await EngineContext.Current.Resolve<IInstallHelper>().GetSupportedWidgetZonesAsync("SevenSpikes.Nop.Plugins.AjaxFilters", storeScope));
        nopAjaxFiltersSettingsModel = model;
        nopAjaxFiltersSettingsModel.AvailableFiltersUIModes = await model.FiltersUIMode.ToSelectListAsync<FiltersUIMode>(true, (int[])null, true);
        model.EnableAjaxFilters = model.EnableAjaxFilters && _widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.AjaxFilters");
        model.IsTrialVersion = false;
    }

    [HttpPost]
    public ActionResult GridList(DataSourceRequest command)
    {
        IList<string> productAttributes = _nopAjaxFiltersSettings.ActiveProductAttributes;
        IList<ProductAttribute> source = new List<ProductAttribute>();
        IDictionary<int, int> dictionary = new Dictionary<int, int>();
        if (productAttributes.Count > 0)
        {
            dictionary = _convertToDictionaryHelper.CreateDictionaryFromSemicolonSeparatedPairs<int, int>(productAttributes);
            source = _productAttributeService.GetProductAttributesByIds(dictionary.Select((KeyValuePair<int, int> x) => x.Key).ToArray());
        }
        return Json(new DataSourceResult
        {
            Data = source.Select(delegate (ProductAttribute x)
            {
                int displayOrder = dictionary[x.Id];
                return new
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayOrder = displayOrder
                };
            }),
            Total = source.Count
        });
    }

    [HttpPost]
    public async Task<ActionResult> Update(DataSourceRequest command, int id, int displayOrder)
    {
        if (id == 0)
        {
            return EmptyJson;
        }
        IList<string> productAttributes1 = _nopAjaxFiltersSettings.ActiveProductAttributes;
        IDictionary<int, int> semicolonSeparatedPairs = _convertToDictionaryHelper.CreateDictionaryFromSemicolonSeparatedPairs<int, int>(productAttributes1);
        if (semicolonSeparatedPairs.ContainsKey(id))
        {
            List<string> activeProductAttributes = _nopAjaxFiltersSettings.ActiveProductAttributes;
            string stringAndClear1 = $"{id}:{semicolonSeparatedPairs[id]}";
            activeProductAttributes.Remove(stringAndClear1);
            List<string> activeProductAttributes2 = _nopAjaxFiltersSettings.ActiveProductAttributes;
            string stringAndClear2 = $"{id}:{displayOrder}";
            activeProductAttributes2.Add(stringAndClear2);
            await _settingService.SaveSettingAsync<NopAjaxFiltersSettings>(_nopAjaxFiltersSettings, 0);
        }
        return GridList(command);
    }

    [HttpPost]
    public async Task<ActionResult> Delete(DataSourceRequest command, int id)
    {
        if (id == 0)
        {
            return EmptyJson;
        }
        IList<string> productAttributes1 = _nopAjaxFiltersSettings.ActiveProductAttributes;
        IDictionary<int, int> semicolonSeparatedPairs = _convertToDictionaryHelper.CreateDictionaryFromSemicolonSeparatedPairs<int, int>(productAttributes1);
        if (semicolonSeparatedPairs.ContainsKey(id))
        {
            List<string> activeProductAttributes = _nopAjaxFiltersSettings.ActiveProductAttributes;
            string stringAndClear = $"{id}:{semicolonSeparatedPairs[id]}";
            activeProductAttributes.Remove(stringAndClear);
            await _settingService.SaveSettingAsync<NopAjaxFiltersSettings>(_nopAjaxFiltersSettings, 0);
        }
        return GridList(command);
    }

    [HttpPost]
    public async Task<ActionResult> Create(DataSourceRequest command, string name, int displayOrder)
    {
        if (!string.IsNullOrEmpty(name))
        {
            ProductAttribute productAttributeByName = _productAttributeService.GetProductAttributeByName(name);
            if (productAttributeByName != null && !_convertToDictionaryHelper.CreateDictionaryFromSemicolonSeparatedPairs<int, int>((IList<string>)_nopAjaxFiltersSettings.ActiveProductAttributes).ContainsKey(productAttributeByName.Id))
            {
                List<string> activeProductAttributes = _nopAjaxFiltersSettings.ActiveProductAttributes;
                string stringAndClear = $"{productAttributeByName.Id}:{displayOrder}";
                activeProductAttributes.Add(stringAndClear);
                await _settingService.SaveSettingAsync<NopAjaxFiltersSettings>(_nopAjaxFiltersSettings, 0);
            }
        }
        return GridList(command);
    }

    [HttpGet]
    public async Task<ActionResult> GetProductAttributes()
    {
        IList<string> productAttributes = this._nopAjaxFiltersSettings.ActiveProductAttributes;
        IEnumerable<int> currentProductAttributeIds = from x in this._convertToDictionaryHelper.CreateDictionaryFromSemicolonSeparatedPairs<int, int>(productAttributes)
                                                      select x.Key;
        IList<SelectListItem> list = (from x in (IEnumerable<ProductAttribute>)(await this._productAttributeService.GetAllProductAttributesAsync(0, int.MaxValue))
                                      where !currentProductAttributeIds.Contains(x.Id)
                                      select new SelectListItem
                                      {
                                          Value = x.Id.ToString(),
                                          Text = x.Name
                                      }).ToList();
        return Json(list);
    }

    [HttpPost]
    public ActionResult SpecificationAttributeSlidersGridList(DataSourceRequest command)
    {
        IList<int> attributeSliders = _nopAjaxFiltersSettings.SpecificationAttributeSliders;
        IList<SpecificationAttribute> source = new List<SpecificationAttribute>();
        if (attributeSliders.Count > 0)
        {
            source = _specificationAttributeService.GetSpecificationAttributeByIds(attributeSliders.ToArray());
        }
        return Json(new DataSourceResult
        {
            Data = source.Select((SpecificationAttribute x) => new
            {
                x.Id,
                x.Name
            }),
            Total = source.Count
        });
    }

    [HttpPost]
    public async Task<ActionResult> SpecificationAttributeSlidersDeleteAsync(DataSourceRequest command, int id)
    {
        if (id == 0)
        {
            return EmptyJson;
        }
        if (this._nopAjaxFiltersSettings.SpecificationAttributeSliders.Contains(id))
        {
            this._nopAjaxFiltersSettings.SpecificationAttributeSliders.Remove(id);
            await this._settingService.SaveSettingAsync<NopAjaxFiltersSettings>(this._nopAjaxFiltersSettings, 0);
        }
        return this.GridList(command);
    }

    [HttpPost]
    public async Task<ActionResult> SpecificationAttributeSlidersCreateAsync(DataSourceRequest command, string name, int displayOrder)
    {
        if (!string.IsNullOrEmpty(name))
        {
            SpecificationAttribute specificationAttributeByName = _specificationAttributeService.GetSpecificationAttributeByName(name);
            if (specificationAttributeByName != null && !_nopAjaxFiltersSettings.SpecificationAttributeSliders.Contains(specificationAttributeByName.Id))
            {
                _nopAjaxFiltersSettings.SpecificationAttributeSliders.Add(specificationAttributeByName.Id);
                await _settingService.SaveSettingAsync<NopAjaxFiltersSettings>(_nopAjaxFiltersSettings, 0);
            }
        }
        return GridList(command);
    }

    [HttpGet]
    public async Task<ActionResult> GetSpecifications()
    {
        IList<int> currentSpecifications = this._nopAjaxFiltersSettings.SpecificationAttributeSliders;
        IList<SelectListItem> list = (from x in (IEnumerable<SpecificationAttribute>)(await this._specificationAttributeService.GetSpecificationAttributesAsync(0, int.MaxValue))
                                      where !currentSpecifications.Contains(x.Id)
                                      select new SelectListItem
                                      {
                                          Value = x.Id.ToString(),
                                          Text = x.Name
                                      }).ToList();
        return Json(list);
    }
}
