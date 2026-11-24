using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Components;

[ViewComponent(Name = "NopAjaxCartPopupCrossSellProducts")]
public class NopAjaxCartPopupCrossSellProductsComponent : ViewComponent
{
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IAclService _aclService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly NopAjaxCartSettings _nopAjaxCartSettings;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IShoppingCartService _shoppingCartService;

    public NopAjaxCartPopupCrossSellProductsComponent(
        IProductModelFactory productModelFactory,
        IProductService productService,
        IAclService aclService,
        IStoreMappingService storeMappingService,
        NopAjaxCartSettings nopAjaxCartSettings,
        IStoreContext storeContext,
        IWorkContext workContext,
        IShoppingCartService shoppingCartService)
    {
        _productModelFactory = productModelFactory;
        _productService = productService;
        _aclService = aclService;
        _storeMappingService = storeMappingService;
        _nopAjaxCartSettings = nopAjaxCartSettings;
        _storeContext = storeContext;
        _workContext = workContext;
        _shoppingCartService = shoppingCartService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId)
    {
        var crossSellProducts = await _productService.GetCrossSellProductsByProductId1Async(
            productId,
            false);

        var shoppingCartProductIds = new HashSet<int>(await GetShoppingCartProductIdsAsync());
        var candidateProductIds = crossSellProducts
            .Where(item => !shoppingCartProductIds.Contains(item.ProductId2))
            .Select(item => item.ProductId2)
            .ToArray();

        var productsToDisplay = new List<Product>();
        var candidateProducts = await _productService.GetProductsByIdsAsync(candidateProductIds);

        foreach (var product in candidateProducts)
        {
            if (!await _aclService.AuthorizeAsync<Product>(product))
                continue;

            if (!await _storeMappingService.AuthorizeAsync<Product>(product))
                continue;

            productsToDisplay.Add(product);

            if (productsToDisplay.Count == _nopAjaxCartSettings.NumberOfCrossSellProductsInPopup)
                break;
        }

        var productModels = await _productModelFactory.PrepareProductOverviewModelsAsync(
            productsToDisplay,
            true,
            true,
            _nopAjaxCartSettings.ProductAddedToCartImageSize,
            false,
            false);

        return View("PopupCrossSellProducts", productModels.ToList());
    }

    private async Task<int[]> GetShoppingCartProductIdsAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(
            customer,
            ShoppingCartType.ShoppingCart,
            store.Id,
            null,
            null,
            null);

        return shoppingCart.Select(item => item.ProductId).ToArray();
    }
}
