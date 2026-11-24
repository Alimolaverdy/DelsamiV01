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
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Controllers;

public class NopAjaxCartCatalogController : BasePublicController
{
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IAclService _aclService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly NopAjaxCartSettings _settings;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IShoppingCartService _shoppingCartService;

    public NopAjaxCartCatalogController(
        IProductModelFactory productModelFactory,
        IProductService productService,
        IProductAttributeService productAttributeService,
        IAclService aclService,
        IStoreMappingService storeMappingService,
        NopAjaxCartSettings settings,
        IStoreContext storeContext,
        IWorkContext workContext,
        IShoppingCartService shoppingCartService)
    {
        _productModelFactory = productModelFactory;
        _productService = productService;
        _productAttributeService = productAttributeService;
        _aclService = aclService;
        _storeMappingService = storeMappingService;
        _settings = settings;
        _storeContext = storeContext;
        _workContext = workContext;
        _shoppingCartService = shoppingCartService;
    }

    public async Task<JsonResult> CheckIfProductOrItsAssociatedProductsHasAttributes(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
        {
            return Json(new { HasProductAttributes = false });
        }

        var showMiniDetailsView = product.IsRental
            || product.ProductType == ProductType.GroupedProduct;

        if (product.ProductType == ProductType.SimpleProduct && !product.IsRental)
            showMiniDetailsView = await CheckIfProductHasProductAttributesAsync(productId, product);

        if (product.HasTierPrices)
            showMiniDetailsView = true;

        if (!_settings.EnableProductQuantityTextBox && product.OrderMinimumQuantity > 1)
            showMiniDetailsView = true;

        return Json(new { HasProductAttributes = showMiniDetailsView });
    }

    public async Task<ActionResult> GetMiniProductDetailsView(
        int productId,
        bool isAddToCartButton = true)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted || !product.Published)
        {
            return new ContentResult
            {
                Content = "Product is either deleted or not published"
            };
        }

        var model = await _productModelFactory.PrepareProductDetailsModelAsync(
            product,
            null,
            false);
        var viewName = await _productModelFactory.PrepareProductTemplateViewPathAsync(product);
        ViewBag.IsAddToCartButton = isAddToCartButton;

        return PartialView(viewName, model);
    }

    public async Task<ActionResult> GetRelatedProducts(int productId)
    {
        var relatedProducts = await _productService.GetRelatedProductsByProductId1Async(
            productId,
            false);
        var relatedProductIds = relatedProducts
            .Take(_settings.NumberOfRelatedProductsInPopup)
            .Select(item => item.ProductId2)
            .ToArray();
        var products = await GetAuthorizedProductsAsync(
            await _productService.GetProductsByIdsAsync(relatedProductIds));

        var models = await _productModelFactory.PrepareProductOverviewModelsAsync(
            products,
            true,
            true,
            _settings.ProductAddedToCartImageSize,
            false,
            false);

        return PartialView("PopupRelatedProducts", models.ToList());
    }

    public async Task<ActionResult> GetCrossSellProducts(int productId)
    {
        var crossSellProducts = await _productService.GetCrossSellProductsByProductId1Async(
            productId,
            false);
        var shoppingCartProductIds = new HashSet<int>(await GetShoppingCartProductIdsAsync());
        var candidateProductIds = crossSellProducts
            .Where(item => !shoppingCartProductIds.Contains(item.ProductId2))
            .Select(item => item.ProductId2)
            .ToArray();

        var products = await GetAuthorizedProductsAsync(
            await _productService.GetProductsByIdsAsync(candidateProductIds),
            _settings.NumberOfCrossSellProductsInPopup);

        var models = await _productModelFactory.PrepareProductOverviewModelsAsync(
            products,
            true,
            true,
            _settings.ProductAddedToCartImageSize,
            false,
            false);

        return PartialView("PopupCrossSellProducts", models.ToList());
    }

    private async Task<List<Product>> GetAuthorizedProductsAsync(
        IEnumerable<Product> candidates,
        int maximumCount = int.MaxValue)
    {
        var products = new List<Product>();
        foreach (var product in candidates)
        {
            if (!await _aclService.AuthorizeAsync<Product>(product)
                || !await _storeMappingService.AuthorizeAsync<Product>(product))
            {
                continue;
            }

            products.Add(product);
            if (products.Count == maximumCount)
                break;
        }

        return products;
    }

    private async Task<bool> CheckIfProductHasProductAttributesAsync(int productId, Product product)
    {
        if (product.IsGiftCard || product.CustomerEntersPrice)
            return true;

        if (!_settings.EnableProductQuantityTextBox
            && _productService.ParseAllowedQuantities(product).Length != 0)
        {
            return true;
        }

        var mappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
        return mappings.Any(mapping => (int)mapping.AttributeControlType != 50);
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
