using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Misc.ShipmentManagement.Factories;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Controllers
{
    public class DelsamiShipmentManagementController : BasePluginController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IDelsamiOrderModelFactory _DelsamiOrderModelFactory;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public DelsamiShipmentManagementController(IOrderService orderService,
                                                    IWorkContext workContext,
                                                    ICustomerService customerService,
                                                    IDelsamiOrderModelFactory DelsamiOrderModelFactory)
        {
            _orderService = orderService;
            _workContext = workContext;
            _DelsamiOrderModelFactory = DelsamiOrderModelFactory;
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

            var model = await _DelsamiOrderModelFactory.PrepareOrderDetailsModelAsync(order);

            return View("~/Plugins/Delsami.Misc.ShipmentManagement/Themes/Traction/Views/DelsamiShipmentManagement/OrderDetails.cshtml", model);
        }

        public virtual async Task<IActionResult> CustomerOrders()
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await _DelsamiOrderModelFactory.PrepareCustomerOrderListModelAsync();
            return View("~/Plugins/Delsami.Misc.ShipmentManagement/Themes/Traction/Views/DelsamiShipmentManagement/CustomerOrders.cshtml", model);
        }

        #endregion
    }
}
