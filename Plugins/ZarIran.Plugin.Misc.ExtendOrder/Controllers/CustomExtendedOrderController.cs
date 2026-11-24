using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CustomExtendedOrderController : BasePluginController
    {
        #region Fields

        private readonly IExtendedOrderService _extendedOrderService;
        private readonly ExtendedOrderSettings _extendedOrderSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;

        #endregion

        #region Ctor

        public CustomExtendedOrderController(IExtendedOrderService extendedOrderService,
                                             ExtendedOrderSettings extendedOrderSettings,
                                             IOrderProcessingService orderProcessingService,
                                             IWorkContext workContext,
                                             IStoreContext storeContext,
                                             IShoppingCartService shoppingCartService)
        {
            _extendedOrderService = extendedOrderService;
            _extendedOrderSettings = extendedOrderSettings;
            _orderProcessingService = orderProcessingService;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Checkout()
        {

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var carts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!carts.Any())
            {
                //EngineContext.Current.Resolve<ILogger>().Information($"GetCurrentCustomerLastOrderAsync{DateTime.Now}");

                var lastOrder = await _extendedOrderService.GetCurrentCustomerLastOrderAsync();

                //EngineContext.Current.Resolve<ILogger>().Information($"GetCurrentCustomerLastOrderAsync{DateTime.Now}");

                if (lastOrder == null || lastOrder.CreatedOnUtc < DateTime.UtcNow.AddMinutes(-_extendedOrderSettings.OrderExpirationAsMinutes))
                    return RedirectToAction("Index", "Home");
                else
                {
                    //EngineContext.Current.Resolve<ILogger>().Information($"CancelOrderAsync{DateTime.Now}");

                    await _orderProcessingService.CancelOrderAsync(lastOrder, false);

                    //EngineContext.Current.Resolve<ILogger>().Information($"CancelOrderAsync{DateTime.Now}");

                    return RedirectToRoute("reorder", new { orderId = lastOrder.Id });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
