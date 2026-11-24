using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.ShoppingCart;
using SevenSpikes.Nop.Plugins.AjaxCart.Controllers;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;
using SevenSpikes.Nop.Plugins.AjaxCart.Models;

public class NopAjaxCartShoppingCartController : ShoppingCartController
{
    private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

    private readonly IStoreContext _storeContext;

    private readonly ILocalizationService _localizationService;

    private readonly ITaxService _taxService;

    private readonly ICurrencyService _currencyService;

    private readonly IPriceFormatter _priceFormatter;

    private readonly IProductService _productService;

    private readonly IShoppingCartService _shoppingCartService;

    private readonly IPermissionService _permissionService;

    private readonly IWorkContext _workContext;

    private readonly ShoppingCartSettings _shoppingCartSettings;

    private readonly IProductAttributeService _productAttributeService;

    private readonly IProductAttributeParser _productAttributeParser;

    private readonly IUrlRecordService _urlRecordService;

    private readonly NopAjaxCartSettings _ajaxCartSettings;

    private readonly IProductAttributeFormatter _productAttributeFormatter;

    public NopAjaxCartShoppingCartController(NopAjaxCartSettings ajaxCartSettings, IProductAttributeFormatter productAttributeFormatter, CaptchaSettings captchaSettings, CustomerSettings customerSettings, ICheckoutAttributeParser checkoutAttributeParser, ICheckoutAttributeService checkoutAttributeService, ICurrencyService currencyService, ICustomerActivityService customerActivityService, ICustomerService customerService, IDiscountService discountService, IDownloadService downloadService, IGenericAttributeService genericAttributeService, IGiftCardService giftCardService, IHtmlFormatter htmlFormatter, ILocalizationService localizationService, INopFileProvider fileProvider, INopUrlHelper nopUrlHelper, INotificationService notificationService, IPermissionService permissionService, IPictureService pictureService, IPriceFormatter priceFormatter, IProductAttributeParser productAttributeParser, IProductAttributeService productAttributeService, IProductService productService, IShippingService shippingService, IShoppingCartModelFactory shoppingCartModelFactory, IShoppingCartService shoppingCartService, IStaticCacheManager staticCacheManager, IStoreContext storeContext, ITaxService taxService, IUrlRecordService urlRecordService, IWebHelper webHelper, IWorkContext workContext, IWorkflowMessageService workflowMessageService, MediaSettings mediaSettings, OrderSettings orderSettings, ShoppingCartSettings shoppingCartSettings, ShippingSettings shippingSettings)
        : base(captchaSettings, customerSettings, checkoutAttributeParser, checkoutAttributeService, currencyService, customerActivityService, customerService, discountService, downloadService, genericAttributeService, giftCardService, htmlFormatter, localizationService, fileProvider, nopUrlHelper, notificationService, permissionService, pictureService, priceFormatter, productAttributeParser, productAttributeService, productService, shippingService, shoppingCartModelFactory, shoppingCartService, staticCacheManager, storeContext, taxService, urlRecordService, webHelper, workContext, workflowMessageService, mediaSettings, orderSettings, shoppingCartSettings, shippingSettings)
    {
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _storeContext = storeContext;
        _localizationService = localizationService;
        _taxService = taxService;
        _currencyService = currencyService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _permissionService = permissionService;
        _workContext = workContext;
        _shoppingCartSettings = shoppingCartSettings;
        _productAttributeService = productAttributeService;
        _productAttributeParser = productAttributeParser;
        _urlRecordService = urlRecordService;
        _ajaxCartSettings = ajaxCartSettings;
        _productAttributeFormatter = productAttributeFormatter;
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<JsonResult> AddProductToCartAjax(int productId, int quantity, bool isAddToCartButton)
    {
        return await AddProductToCartAjaxInternalAsync(productId, quantity, isAddToCartButton);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<JsonResult> AddProductFromProductDetailsPageToCartAjax(int productId, bool isAddToCartButton, IFormCollection form)
    {
        return await AddProductFromProductDetailsPageToCartAjaxInternalAsync(productId, isAddToCartButton, form);
    }

    public async Task<ActionResult> MiniShoppingCart()
    {
        if (!_shoppingCartSettings.MiniShoppingCartEnabled)
        {
            return Content("");
        }
        if (!(await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart)))
        {
            return Content("");
        }
        return PartialView((await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync()));
    }

    public ActionResult NopAjaxCartFlyoutShoppingCart()
    {
        return ViewComponent("FlyoutShoppingCart");
    }

    public async Task<JsonResult> AddProductsToCart_Details(IFormCollection form)
    {
        List<AddToCartResult> messages = new List<AddToCartResult>();
        if (!form.TryGetValue("productIds", out var value))
        {
            messages.Add(new AddToCartResult
            {
                Success = false,
                ProductId = 0,
                Message = "No products can be added to cart"
            });
        }
        string[] array = value.ToString().Split(',');
        string[] array2 = array;
        foreach (string text in array2)
        {
            string key = $"addtocart_{text}.EnteredQuantity";
            if (form.TryGetValue(key, out var value2) && int.TryParse(value2, out var result) && int.TryParse(text, out var productId) && result > 0)
            {
                AddProductToCartResultModel addProductToCartResultModel = JsonConvert.DeserializeObject<AddProductToCartResultModel>(JsonConvert.SerializeObject((await AddProductFromProductDetailsPageToCartAjaxInternalAsync(productId, isAddToCartButton: true, form)).Value));
                if (!addProductToCartResultModel.Status.Equals("success"))
                {
                    messages.Add(new AddToCartResult
                    {
                        Success = false,
                        ProductId = productId,
                        Message = addProductToCartResultModel.AddToCartWarnings
                    });
                }
                else
                {
                    messages.Add(new AddToCartResult
                    {
                        Success = true,
                        ProductId = productId,
                        Message = addProductToCartResultModel.PopupTitle
                    });
                }
            }
        }
        return Json(new
        {
            result = messages
        });
    }

    private async Task<MiniShoppingCartModel> GetShoppingCartItemModelAsync(int productId, int quantity)
    {
        var model = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();
        var product = await _productService.GetProductByIdAsync(productId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
        {
            ProductId = productId,
            ProductName = await _localizationService.GetLocalizedAsync<Product, string>(
                product,
                item => item.Name,
                null,
                true,
                true),
            ProductSeName = await _urlRecordService.GetSeNameAsync<Product>(product, null, true, true),
            Quantity = quantity
        };

        var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(
            customer,
            ShoppingCartType.ShoppingCart,
            store.Id,
            productId,
            null,
            null);
        var shoppingCartItem = shoppingCart
            .OrderByDescending(item => item.UpdatedOnUtc)
            .FirstOrDefault();

        if (shoppingCartItem == null)
            return new MiniShoppingCartModel();

        var unitPrice = await _shoppingCartService.GetUnitPriceAsync(shoppingCartItem, true);
        var taxAdjustedPrice = await _taxService.GetProductPriceAsync(product, unitPrice.Item1);
        var currency = await _workContext.GetWorkingCurrencyAsync();
        var convertedPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
            taxAdjustedPrice.Item1,
            currency);

        cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(convertedPrice);
        cartItemModel.Id = shoppingCartItem.Id;
        cartItemModel.Picture = await _shoppingCartModelFactory.PrepareCartItemPictureModelAsync(
            shoppingCartItem,
            _ajaxCartSettings.ProductAddedToCartImageSize,
            true,
            cartItemModel.ProductName);
        cartItemModel.AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(
            product,
            shoppingCartItem.AttributesXml);

        model.Items.Clear();
        model.Items.Add(cartItemModel);
        return model;
    }

    private async Task<JsonResult> AddProductToCartAjaxInternalAsync(
        int productId,
        int quantity,
        bool isAddToCartButton)
    {
        var result = new AddProductToCartResultModel
        {
            Status = "success",
            AddToCartWarnings = string.Empty,
            ErrorMessage = string.Empty,
            RedirectUrl = string.Empty
        };

        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
        {
            result.Status = "error";
            result.PopupTitle = await _localizationService.GetResourceAsync(
                "SevenSpikes.NopAjaxCart.PopupTitle.Error");
            result.ErrorMessage = $"Product with Id {productId} cannot be found";
            return Json(result);
        }

        var attributeMappings = await _productAttributeService
            .GetProductAttributeMappingsByProductIdAsync(productId);
        var attributesXml = string.Empty;

        foreach (var attributeMapping in attributeMappings)
        {
            var attributeValues = await _productAttributeService
                .GetProductAttributeValuesAsync(attributeMapping.Id);

            foreach (var attributeValue in attributeValues.Where(value => value.IsPreSelected))
            {
                attributesXml = _productAttributeParser.AddProductAttribute(
                    attributesXml,
                    attributeMapping,
                    attributeValue.Id.ToString(),
                    null);
            }
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cartType = isAddToCartButton
            ? ShoppingCartType.ShoppingCart
            : ShoppingCartType.Wishlist;

        var warnings = await _shoppingCartService.AddToCartAsync(
            customer,
            product,
            cartType,
            store.Id,
            attributesXml,
            0m,
            null,
            null,
            quantity,
            true);

        if (warnings.Count == 0)
        {
            result.ProductAddedToCartWindow = await ProductAddedToCartWindowResultAsync(
                product,
                quantity,
                isAddToCartButton);
            return Json(result);
        }

        var warningList = new StringBuilder("<ul>");
        foreach (var warning in warnings)
            warningList.AppendFormat("<li>{0}</li>", warning);
        warningList.Append("</ul>");

        result.Status = "warning";
        result.PopupTitle = await _localizationService.GetResourceAsync(
            "SevenSpikes.NopAjaxCart.PopupTitle.Warning");
        result.AddToCartWarnings = warningList.ToString();

        return Json(result);
    }

    private async Task<string> ProductAddedToCartWindowResultAsync(
        Product product,
        int quantity,
        bool isAddToCartButton)
    {
        if (isAddToCartButton)
        {
            var model = new ProductAddedToCartPopupDialogModel
            {
                MiniShoppingCart = await GetShoppingCartItemModelAsync(product.Id, quantity),
                EnableRelatedProductsInPopup = _ajaxCartSettings.EnableRelatedProductsInPopup,
                EnableCrossSellProductsInPopup = _ajaxCartSettings.EnableCrossSellProductsInPopup
            };

            return await RenderPartialViewToStringAsync("ProductAddedToCartPopupDialog", model);
        }

        var wishlistModel = new ProductAddedToWishlistPopupDialogModel
        {
            WishlistShoppingCartItemModel = await PrepareWishlistShoppingCartItemModelAsync(product, quantity),
            EnableRelatedProductsInPopup = _ajaxCartSettings.EnableRelatedProductsInPopup,
            EnableCrossSellProductsInPopup = _ajaxCartSettings.EnableCrossSellProductsInPopup
        };

        return await RenderPartialViewToStringAsync("ProductAddedToWishlistPopupDialog", wishlistModel);
    }

    private async Task<WishlistModel.ShoppingCartItemModel> PrepareWishlistShoppingCartItemModelAsync(
        Product product,
        int quantity)
    {
        var model = new WishlistModel.ShoppingCartItemModel
        {
            ProductId = product.Id,
            ProductName = await _localizationService.GetLocalizedAsync<Product, string>(
                product,
                item => item.Name,
                null,
                true,
                true),
            ProductSeName = await _urlRecordService.GetSeNameAsync<Product>(product, null, true, true),
            Quantity = quantity
        };

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(
            customer,
            ShoppingCartType.Wishlist,
            store.Id,
            product.Id,
            null,
            null);
        var shoppingCartItem = shoppingCart
            .OrderByDescending(item => item.UpdatedOnUtc)
            .FirstOrDefault();

        if (shoppingCartItem == null)
            return model;

        var unitPrice = await _shoppingCartService.GetUnitPriceAsync(shoppingCartItem, true);
        var taxAdjustedPrice = await _taxService.GetProductPriceAsync(product, unitPrice.Item1);
        var currency = await _workContext.GetWorkingCurrencyAsync();
        var convertedPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
            taxAdjustedPrice.Item1,
            currency);

        model.UnitPrice = await _priceFormatter.FormatPriceAsync(convertedPrice);
        model.Picture = await _shoppingCartModelFactory.PrepareCartItemPictureModelAsync(
            shoppingCartItem,
            _ajaxCartSettings.ProductAddedToCartImageSize,
            true,
            model.ProductName);
        model.AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(
            product,
            shoppingCartItem.AttributesXml);

        return model;
    }

    private async Task<JsonResult> AddProductFromProductDetailsPageToCartAjaxInternalAsync(
        int productId,
        bool isAddToCartButton,
        IFormCollection form)
    {
        var cartType = isAddToCartButton
            ? ShoppingCartType.ShoppingCart
            : ShoppingCartType.Wishlist;
        var actionResult = await AddProductToCart_Details(productId, (int)cartType, form);

        if (actionResult is not JsonResult jsonResult)
            return new NullJsonResult();

        var result = new AddProductToCartResultModel
        {
            Status = "success",
            AddToCartWarnings = string.Empty,
            ErrorMessage = string.Empty,
            RedirectUrl = string.Empty
        };

        var cartResult = JsonConvert.DeserializeObject<AddProductVariantToCartResultModel>(
            JsonConvert.SerializeObject(jsonResult.Value));

        if (cartResult.success)
        {
            var quantity = 1;
            var quantityKey = $"addtocart_{productId}.EnteredQuantity";
            if (form.TryGetValue(quantityKey, out var quantityValue)
                && int.TryParse(quantityValue, out var parsedQuantity))
            {
                quantity = parsedQuantity;
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                result.Status = "error";
                result.PopupTitle = await _localizationService.GetResourceAsync(
                    "SevenSpikes.NopAjaxCart.PopupTitle.Error");
                result.ErrorMessage = $"Product with Id {productId} cannot be found";
                return Json(result);
            }

            result.ProductAddedToCartWindow = await ProductAddedToCartWindowResultAsync(
                product,
                quantity,
                isAddToCartButton);
            return Json(result);
        }

        if (cartResult.message is string message)
        {
            result.Status = "error";
            result.PopupTitle = await _localizationService.GetResourceAsync(
                "SevenSpikes.NopAjaxCart.PopupTitle.Error");
            result.AddToCartWarnings = message;
        }
        else if (cartResult.message is JArray warnings)
        {
            var warningList = new StringBuilder("<ul>");
            foreach (var warning in warnings)
                warningList.AppendFormat("<li>{0}</li>", (string)warning);
            warningList.Append("</ul>");

            result.Status = "warning";
            result.PopupTitle = await _localizationService.GetResourceAsync(
                "SevenSpikes.NopAjaxCart.PopupTitle.Warning");
            result.AddToCartWarnings = warningList.ToString();
        }

        return Json(result);
    }

}
