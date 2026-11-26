using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Misc.ShoppingCartPage.Factories;
using Delsami.Plugin.Misc.ShoppingCartPage.Models;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartPage.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class ShoppingCartController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IDelsamiShoppingCartModelFactory _DelsamiShoppingCartModelFactory;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;

        #endregion

        #region Ctor

        public ShoppingCartController(IPermissionService permissionService,
                                      IWorkContext workContext,
                                      IStoreContext storeContext,
                                      IShoppingCartService shoppingCartService,
                                      IProductService productService,
                                      IDelsamiShoppingCartModelFactory DelsamiShoppingCartModelFactory,
                                      IPriceFormatter priceFormatter,
                                      ITaxService taxService,
                                      ICurrencyService currencyService)
        {
            _permissionService = permissionService;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _DelsamiShoppingCartModelFactory = DelsamiShoppingCartModelFactory;
            _priceFormatter = priceFormatter;
            _taxService = taxService;
            _currencyService = currencyService;

        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> CustomCart()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
            var model = new DelsamiShoppingCartModel();
            model = await _DelsamiShoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            return View("~/Plugins/Delsami.Misc.ShoppingCartPage/Views/ShoppingCart/Cart.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("Homepage");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var shoppingCartItem = cart.FirstOrDefault(w => w.ProductId == productId);

            if (shoppingCartItem == null)
            {

                return new JsonResult(new
                {
                    IsSucceed = false,
                    ForceRefresh = true,
                    Warnings = new List<string> { }
                });
            }
            var product = await _productService.GetProductByIdAsync(productId);
            var itemWithNewQuantity = new
            {
                NewQuantity = quantity,
                Item = shoppingCartItem,
                Product = product
            };

            //try to update cart items with new quantities and get warnings
            var warnings = await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                    itemWithNewQuantity.Item.Id, itemWithNewQuantity.Item.AttributesXml, itemWithNewQuantity.Item.CustomerEnteredPrice,
                    itemWithNewQuantity.Item.RentalStartDateUtc, itemWithNewQuantity.Item.RentalEndDateUtc, itemWithNewQuantity.NewQuantity, true);


            cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            shoppingCartItem = cart.FirstOrDefault(w => w.ProductId == productId);

            decimal subTotal = 0;
            decimal shoppingCartItemSubTotalWithDiscount = 0;

            if (shoppingCartItem != null)
            {
                (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(shoppingCartItem, true);

                var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, subTotal);
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                shoppingCartItemSubTotalWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
            }

            return new JsonResult(new
            {
                IsSucceed = !warnings.Any(),
                Warnings = warnings,
                OrderTotals = await RenderViewComponent("OrderTotals", null),
                FlayoutShopping = await RenderViewComponent("FlyoutShoppingCart", null),
                TotalQuantity = cart.Sum(x => x.Quantity),
                SubTotal = await _priceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount)
            });
        }

        #endregion

        #region Utilties

        private async Task<string> RenderViewComponent(string viewComponent, object args)
        {
            var sp = HttpContext.RequestServices;

            var helper = new DefaultViewComponentHelper(
                sp.GetRequiredService<IViewComponentDescriptorCollectionProvider>(),
                HtmlEncoder.Default,
                sp.GetRequiredService<IViewComponentSelector>(),
                sp.GetRequiredService<IViewComponentInvokerFactory>(),
                sp.GetRequiredService<IViewBufferScope>());

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, NullView.Instance, ViewData, TempData, writer, new HtmlHelperOptions());
                helper.Contextualize(context);
                var result = await helper.InvokeAsync(viewComponent, args);
                result.WriteTo(writer, HtmlEncoder.Default);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }

        #endregion
    }
}
