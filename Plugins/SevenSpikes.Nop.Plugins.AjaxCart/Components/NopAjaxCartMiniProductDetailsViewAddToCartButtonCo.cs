using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Plugins.AjaxCart.Models;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Components;

[ViewComponent(Name = "NopAjaxCartMiniProductDetailsViewAddToCartButton")]
public class NopAjaxCartMiniProductDetailsViewAddToCartButtonComponent : ViewComponent
{
    private readonly ILocalizationService _localizationService;

    public NopAjaxCartMiniProductDetailsViewAddToCartButtonComponent(
        ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(
        ProductDetailsModel.AddToCartModel addToCartModel)
    {
        var model = new AddProductToCartAjaxButtonModel
        {
            ProductId = addToCartModel.ProductId,
            ButtonValue = await _localizationService.GetResourceAsync(
                addToCartModel.IsRental
                    ? "ShoppingCart.Rent"
                    : "ShoppingCart.AddToCart")
        };

        return View("Default", model);
    }
}
