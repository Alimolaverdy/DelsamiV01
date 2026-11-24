using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;
using System;
using System.Linq.Expressions;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Data.Migrations.UpgradeTo440
{
    [NopMigration("2023/07/11 14:38:00", "SevenSpikes.Nop.Plugins.AjaxCart Selector schema")]
    public class SelectorMigration : MigrationBase
    {
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;
            ISettingService isettingService = EngineContext.Current.Resolve<ISettingService>();
            NopAjaxCartSettings result = isettingService.LoadSettingAsync<NopAjaxCartSettings>(0).GetAwaiter().GetResult();
            if (isettingService.SettingExistsAsync<NopAjaxCartSettings, string>(result, settings => settings.AddToWishlistButtonSelector, 0).GetAwaiter().GetResult() && result.AddToWishlistButtonSelector == "input.add-to-wishlist-button")
            {
                result.AddToWishlistButtonSelector = ".add-to-wishlist-button";
                isettingService.SaveSettingAsync<NopAjaxCartSettings>(result, 0).GetAwaiter().GetResult();
            }
            if (isettingService.SettingExistsAsync<NopAjaxCartSettings, string>(result, settings => settings.ProductPageAddToCartButtonSelector, 0).GetAwaiter().GetResult() && result.ProductPageAddToCartButtonSelector == "input.add-to-cart-button")
            {
                result.ProductPageAddToCartButtonSelector = ".add-to-cart-button";
                isettingService.SaveSettingAsync<NopAjaxCartSettings>(result, 0).GetAwaiter().GetResult();
            }
            if (!isettingService.SettingExistsAsync<NopAjaxCartSettings, string>(result, settings => settings.ProductBoxAddToCartButtonSelector, 0).GetAwaiter().GetResult() || !(result.ProductBoxAddToCartButtonSelector == "input.product-box-add-to-cart-button"))
                return;
            result.ProductBoxAddToCartButtonSelector = ".product-box-add-to-cart-button";
            isettingService.SaveSettingAsync<NopAjaxCartSettings>(result, 0).GetAwaiter().GetResult();
        }

        public override void Down()
        {
        }
    }
}
