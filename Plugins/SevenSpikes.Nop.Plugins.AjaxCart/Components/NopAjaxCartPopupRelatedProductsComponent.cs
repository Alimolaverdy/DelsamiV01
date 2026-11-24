using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Components;

[ViewComponent(Name = "NopAjaxCartPopupRelatedProducts")]
public class NopAjaxCartPopupRelatedProductsComponent : ViewComponent
{
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IAclService _aclService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly NopAjaxCartSettings _settings;

    public NopAjaxCartPopupRelatedProductsComponent(
        IProductModelFactory productModelFactory,
        IProductService productService,
        IAclService aclService,
        IStoreMappingService storeMappingService,
        NopAjaxCartSettings settings)
    {
        _productModelFactory = productModelFactory;
        _productService = productService;
        _aclService = aclService;
        _storeMappingService = storeMappingService;
        _settings = settings;
    }

    public async Task<IViewComponentResult> InvokeAsync(int productId)
    {
        var relatedProducts = await _productService.GetRelatedProductsByProductId1Async(
            productId,
            false);
        var relatedProductIds = relatedProducts
            .Take(_settings.NumberOfRelatedProductsInPopup)
            .Select(item => item.ProductId2)
            .ToArray();
        var candidateProducts = await _productService.GetProductsByIdsAsync(relatedProductIds);
        var products = new List<Product>();

        foreach (var product in candidateProducts)
        {
            if (!await _aclService.AuthorizeAsync<Product>(product)
                || !await _storeMappingService.AuthorizeAsync<Product>(product))
            {
                continue;
            }

            products.Add(product);
        }

        var models = await _productModelFactory.PrepareProductOverviewModelsAsync(
            products,
            true,
            true,
            _settings.ProductAddedToCartImageSize,
            false,
            false);

        return View("PopupRelatedProducts", models.ToList());
    }
}
