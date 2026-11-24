using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Framework.Routing;
using SevenSpikes.Nop.Framework.ViewLocations;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;

public class RouteProvider : BaseRouteProvider
{
    protected override string PluginSystemName => "SevenSpikes.Nop.Plugins.AjaxCart";

    protected override async Task SetNopcommerceSettingsAsync()
    {
        var shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
        var settingService = EngineContext.Current.Resolve<ISettingService>();
        var ajaxCartSettings = EngineContext.Current.Resolve<NopAjaxCartSettings>();

        if (!ajaxCartSettings.EnableNopAjaxCart
            || (!shoppingCartSettings.DisplayWishlistAfterAddingProduct
                && !shoppingCartSettings.DisplayCartAfterAddingProduct))
        {
            return;
        }

        shoppingCartSettings.DisplayCartAfterAddingProduct = false;
        shoppingCartSettings.DisplayWishlistAfterAddingProduct = false;
        await settingService.SaveSettingAsync(shoppingCartSettings, 0);
    }

    protected override void RegisterDuplicateControllers(IViewLocationsManager viewLocationsManager)
    {
        var duplicateControllers = new List<DuplicateControllerInfo>
        {
            new()
            {
                DuplicateControllerName = "NopAjaxCartShoppingCart",
                DuplicateOfControllerName = "ShoppingCart"
            },
            new()
            {
                DuplicateControllerName = "NopAjaxCartCatalog",
                DuplicateOfControllerName = "Product"
            }
        };

        viewLocationsManager.AddDuplicateControllers(duplicateControllers);
    }

    protected override async Task RegisterRoutesAccessibleByNameAsync(IEndpointRouteBuilder routes)
    {
        await base.RegisterRoutesAccessibleByNameAsync(routes);
        var languagePattern = await GetRouteLanguagePatternAsync(routes);

        routes.MapControllerRoute(
            "AddProductFromProductDetailsPageToCartAjax",
            languagePattern + "AddProductFromProductDetailsPageToCartAjax/",
            new
            {
                controller = "NopAjaxCartShoppingCart",
                action = "AddProductFromProductDetailsPageToCartAjax"
            });

        routes.MapControllerRoute(
            "AddMultipleProductsToCart",
            languagePattern + "AddMultipleProductsToCart/",
            new
            {
                controller = "NopAjaxCartShoppingCart",
                action = "AddProductsToCart_Details"
            });

        routes.MapControllerRoute(
            "AddProductToCartAjax",
            languagePattern + "AddProductToCartAjax/",
            new
            {
                controller = "NopAjaxCartShoppingCart",
                action = "AddProductToCartAjax"
            });

        routes.MapControllerRoute(
            "MiniShoppingCart",
            languagePattern + "MiniShoppingCart/",
            new
            {
                controller = "NopAjaxCartShoppingCart",
                action = "MiniShoppingCart"
            });

        routes.MapControllerRoute(
            "NopAjaxCartFlyoutShoppingCart",
            languagePattern + "NopAjaxCartFlyoutShoppingCart/",
            new
            {
                controller = "NopAjaxCartShoppingCart",
                action = "NopAjaxCartFlyoutShoppingCart"
            });

        routes.MapControllerRoute(
            "CheckIfProductOrItsAssociatedProductsHasAttributes",
            languagePattern + "CheckIfProductOrItsAssociatedProductsHasAttributes/",
            new
            {
                controller = "NopAjaxCartCatalog",
                action = "CheckIfProductOrItsAssociatedProductsHasAttributes"
            });

        routes.MapControllerRoute(
            "GetMiniProductDetailsView",
            languagePattern + "GetMiniProductDetailsView/",
            new
            {
                controller = "NopAjaxCartCatalog",
                action = "GetMiniProductDetailsView"
            });
    }

    protected override bool ShouldAddPluginViewLocationsBeforeNopViewLocations()
    {
        return true;
    }
}
