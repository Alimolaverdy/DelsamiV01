using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Framework.Controllers;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;
using SevenSpikes.Nop.Plugins.AjaxCart.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.AjaxCart.Models;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Controllers;

public class NopAjaxCartController : Base7SpikesPublicController
{
    private readonly NopAjaxCartSettings _settings;
    private readonly ILocalizationService _localizationService;
    private readonly IProductService _productService;
    private readonly IStoreContext _storeContext;
    private readonly IEncryptionService _encryptionService;
    private readonly IStaticCacheManager _staticCacheManager;

    public NopAjaxCartController(
        NopAjaxCartSettings settings,
        ILocalizationService localizationService,
        IProductService productService,
        IStoreContext storeContext,
        IEncryptionService encryptionService,
        IStaticCacheManager staticCacheManager)
    {
        _settings = settings;
        _localizationService = localizationService;
        _productService = productService;
        _storeContext = storeContext;
        _encryptionService = encryptionService;
        _staticCacheManager = staticCacheManager;
    }

    [HttpGet]
    public ActionResult GetAjaxCartButtonsAjax()
    {
        return new EmptyResult();
    }

    [HttpPost]
    public async Task<ActionResult> GetAjaxCartButtonsAjax(
        [FromBody] List<AddToCartButtonInfoModel> addToCartButtonsInfos)
    {
        if (addToCartButtonsInfos == null || addToCartButtonsInfos.Count == 0)
            return new EmptyResult();

        var model = new AjaxCartButtonsModel();
        var buttonSignature = string.Join(
            ",",
            addToCartButtonsInfos.Select(button =>
                $"{button.ProductId},{button.IsProductPage},{button.ButtonValue}"));
        var signatureHash = _encryptionService.CreatePasswordHash(
            buttonSignature,
            "SS_AjaxCart_EK_1",
            "SHA1");
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            CacheKeys.AjaxCartButtonsModelCacheKey,
            signatureHash,
            storeId,
            _settings.EnableOnCatalogPages,
            _settings.EnableOnProductPage);

        model.AddProductToCartAjaxButtonModels = await _staticCacheManager.GetAsync(
            cacheKey,
            async () =>
            {
                var buttonModels = new List<AddProductToCartAjaxButtonModel>();
                var catalogButtons = addToCartButtonsInfos
                    .Where(button => !button.IsProductPage)
                    .ToList();

                var productsById = new Dictionary<int, Product>();
                if (catalogButtons.Count > 0 && _settings.EnableOnCatalogPages)
                {
                    var productIds = catalogButtons.Select(button => button.ProductId).Distinct().ToArray();
                    var products = await _productService.GetProductsByIdsAsync(productIds);
                    productsById = products.ToDictionary(product => product.Id);
                }

                foreach (var button in addToCartButtonsInfos)
                {
                    if (button.IsProductPage)
                    {
                        if (_settings.EnableOnProductPage)
                            buttonModels.Add(await CreateAddProductToCartAjaxButtonModelAsync(button));

                        continue;
                    }

                    if (!_settings.EnableOnCatalogPages)
                        continue;

                    if (!productsById.TryGetValue(button.ProductId, out var product))
                        continue;

                    var buttonModel = await CreateAddProductToCartAjaxButtonModelAsync(button);
                    var defaultMinimumQuantity = 1;

                    if (product.ProductType == ProductType.GroupedProduct)
                    {
                        var associatedProducts = await _productService.GetAssociatedProductsAsync(
                            product.Id,
                            storeId,
                            0,
                            false);
                        var firstAssociatedProduct = associatedProducts.FirstOrDefault();

                        if (firstAssociatedProduct != null)
                        {
                            defaultMinimumQuantity = firstAssociatedProduct.OrderMinimumQuantity;
                            product = firstAssociatedProduct;
                        }
                    }
                    else if (product.ProductType == ProductType.SimpleProduct)
                    {
                        defaultMinimumQuantity = product.OrderMinimumQuantity;
                    }

                    foreach (var allowedQuantity in _productService.ParseAllowedQuantities(product))
                    {
                        buttonModel.AllowedQuantities.Add(new SelectListItem
                        {
                            Text = allowedQuantity.ToString(),
                            Value = allowedQuantity.ToString()
                        });
                    }

                    buttonModel.DefaultProductMinimumQuantity = defaultMinimumQuantity;
                    buttonModels.Add(buttonModel);
                }

                return buttonModels;
            });

        return PartialView("AddProductToCartAjaxButton", model);
    }

    private async Task<AddProductToCartAjaxButtonModel> CreateAddProductToCartAjaxButtonModelAsync(
        AddToCartButtonInfoModel button)
    {
        var model = new AddProductToCartAjaxButtonModel
        {
            IsProductPage = button.IsProductPage,
            ButtonValue = button.ButtonValue,
            ProductId = button.ProductId
        };

        if (string.IsNullOrEmpty(model.ButtonValue))
        {
            model.ButtonValue = await _localizationService.GetResourceAsync(
                "ShoppingCart.AddToCart");
        }

        return model;
    }

    public async Task<ActionResult> GetMiniProductDetailsViewAddProductToCartAjaxButton(
        ProductDetailsModel.AddToCartModel addToCartModel)
    {
        var buttonValue = addToCartModel.IsRental
            ? await _localizationService.GetResourceAsync("ShoppingCart.Rent")
            : addToCartModel.AvailableForPreOrder
                ? await _localizationService.GetResourceAsync("ShoppingCart.PreOrder")
                : await _localizationService.GetResourceAsync("ShoppingCart.AddToCart");

        var model = new AddProductToCartAjaxButtonModel
        {
            ProductId = addToCartModel.ProductId,
            ButtonValue = buttonValue
        };

        return PartialView("Default", model);
    }
}
