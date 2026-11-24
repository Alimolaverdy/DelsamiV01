using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.ShipmentManagement.Factories;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Controllers
{
    public class ZarIranShipmentManagementController : BasePluginController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IZarIranOrderModelFactory _ZarIranOrderModelFactory;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public ZarIranShipmentManagementController(IOrderService orderService,
                                                    IWorkContext workContext,
                                                    ICustomerService customerService,
                                                    IZarIranOrderModelFactory ZarIranOrderModelFactory)
        {
            _orderService = orderService;
            _workContext = workContext;
            _ZarIranOrderModelFactory = ZarIranOrderModelFactory;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            var model = await _ZarIranOrderModelFactory.PrepareOrderDetailsModelAsync(order);

            return View("~/Plugins/ZarIran.Misc.ShipmentManagement/Themes/Traction/Views/ZarIranShipmentManagement/OrderDetails.cshtml", model);
        }

        public virtual async Task<IActionResult> CustomerOrders()
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await _ZarIranOrderModelFactory.PrepareCustomerOrderListModelAsync();
            return View("~/Plugins/ZarIran.Misc.ShipmentManagement/Themes/Traction/Views/ZarIranShipmentManagement/CustomerOrders.cshtml", model);
        }

        #endregion
    }
}
