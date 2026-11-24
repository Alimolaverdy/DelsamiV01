using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using SevenSpikes.Nop.Framework.Areas.Admin.ControllerAttributes;
using SevenSpikes.Nop.Framework.Areas.Admin.Controllers;
using SevenSpikes.Nop.Framework.Areas.Admin.Helpers;
using SevenSpikes.Nop.Plugins.AjaxCart.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.AjaxCart.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Areas.Admin.Controllers;

[ManagePluginsAdminAuthorize("SevenSpikes.Nop.Plugins.AjaxCart", false)]
public class NopAjaxCartAdminController : Base7SpikesAdminController
{
    private const string PluginSystemName = "SevenSpikes.Nop.Plugins.AjaxCart";

    private readonly WidgetSettings _widgetSettings;
    private readonly ISettingService _settingService;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly ILocalizationService _localizationService;

    public NopAjaxCartAdminController(
        WidgetSettings widgetSettings,
        ISettingService settingService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService)
    {
        _widgetSettings = widgetSettings;
        _settingService = settingService;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
    }

    public async Task<ActionResult> Settings()
    {
        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<NopAjaxCartSettings>(storeScope);
        var model = settings.ToModel();

        model.ActiveStoreScopeConfiguration = storeScope;
        model.EnableNopAjaxCart = model.EnableNopAjaxCart
            && _widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName);

        if (storeScope > 0)
        {
            var storeSettings = new StoreScopeSettingsHelper<NopAjaxCartSettings>(
                settings,
                storeScope,
                _settingService);

            model.EnableNopAjaxCart_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableNopAjaxCart);
            model.EnableOnMobileDevices_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableOnMobileDevices);
            model.EnableOnProductPage_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableOnProductPage);
            model.EnableOnCatalogPages_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableOnCatalogPages);
            model.EnableProductQuantityTextBox_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableProductQuantityTextBox);
            model.WishlistMenuLinkSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.WishlistMenuLinkSelector);
            model.AddToWishlistButtonSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.AddToWishlistButtonSelector);
            model.ShoppingCartMenuLinkSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.ShoppingCartMenuLinkSelector);
            model.FlyoutCartPanelSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.FlyoutCartPanelSelector);
            model.ProductPageAddToCartButtonSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.ProductPageAddToCartButtonSelector);
            model.ProductBoxAddToCartButtonSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.ProductBoxAddToCartButtonSelector);
            model.ProductBoxProductItemElementSelector_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.ProductBoxProductItemElementSelector);
            model.ProductAddedToCartImageSize_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.ProductAddedToCartImageSize);
            model.EnableRelatedProductsInPopup_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableRelatedProductsInPopup);
            model.NumberOfRelatedProductsInPopup_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.NumberOfRelatedProductsInPopup);
            model.EnableCrossSellProductsInPopup_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableCrossSellProductsInPopup);
            model.NumberOfCrossSellProductsInPopup_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.NumberOfCrossSellProductsInPopup);
        }

        model.IsTrialVersion = false;
        return View(nameof(Settings), model);
    }

    [HttpPost]
    public async Task<ActionResult> Settings(NopAjaxCartSettingsModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Settings));

        if (model.EnableNopAjaxCart && !_widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginSystemName);
            await _settingService.SaveSettingAsync(_widgetSettings, 0);
        }

        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var storeSettings = new StoreScopeSettingsHelper<NopAjaxCartSettings>(
            model.ToEntity(),
            storeScope,
            _settingService);

        await storeSettings.SaveStoreSettingAsync(
            model.EnableNopAjaxCart_OverrideForStore,
            setting => setting.EnableNopAjaxCart);
        await storeSettings.SaveStoreSettingAsync(
            model.EnableOnMobileDevices_OverrideForStore,
            setting => setting.EnableOnMobileDevices);
        await storeSettings.SaveStoreSettingAsync(
            model.EnableOnProductPage_OverrideForStore,
            setting => setting.EnableOnProductPage);
        await storeSettings.SaveStoreSettingAsync(
            model.EnableOnCatalogPages_OverrideForStore,
            setting => setting.EnableOnCatalogPages);
        await storeSettings.SaveStoreSettingAsync(
            model.EnableProductQuantityTextBox_OverrideForStore,
            setting => setting.EnableProductQuantityTextBox);
        await storeSettings.SaveStoreSettingAsync(
            model.WishlistMenuLinkSelector_OverrideForStore,
            setting => setting.WishlistMenuLinkSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.AddToWishlistButtonSelector_OverrideForStore,
            setting => setting.AddToWishlistButtonSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.ShoppingCartMenuLinkSelector_OverrideForStore,
            setting => setting.ShoppingCartMenuLinkSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.FlyoutCartPanelSelector_OverrideForStore,
            setting => setting.FlyoutCartPanelSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.ProductPageAddToCartButtonSelector_OverrideForStore,
            setting => setting.ProductPageAddToCartButtonSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.ProductBoxAddToCartButtonSelector_OverrideForStore,
            setting => setting.ProductBoxAddToCartButtonSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.ProductBoxProductItemElementSelector_OverrideForStore,
            setting => setting.ProductBoxProductItemElementSelector);
        await storeSettings.SaveStoreSettingAsync(
            model.ProductAddedToCartImageSize_OverrideForStore,
            setting => setting.ProductAddedToCartImageSize);
        await storeSettings.SaveStoreSettingAsync(
            model.EnableRelatedProductsInPopup_OverrideForStore,
            setting => setting.EnableRelatedProductsInPopup);
        await storeSettings.SaveStoreSettingAsync(
            model.NumberOfRelatedProductsInPopup_OverrideForStore,
            setting => setting.NumberOfRelatedProductsInPopup);
        await storeSettings.SaveStoreSettingAsync(
            model.EnableCrossSellProductsInPopup_OverrideForStore,
            setting => setting.EnableCrossSellProductsInPopup);
        await storeSettings.SaveStoreSettingAsync(
            model.NumberOfCrossSellProductsInPopup_OverrideForStore,
            setting => setting.NumberOfCrossSellProductsInPopup);

        await _settingService.ClearCacheAsync();
        await _customerActivityService.InsertActivityAsync(
            "EditNopAjaxCartSettings",
            "Edit Nop Ajax Cart Settings",
            null);

        SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));
        SaveSelectedTabName(string.Empty, true);

        return RedirectToAction(nameof(Settings));
    }
}
