using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Controllers
{
    public class DelsamiShoppingCartManagementController : ShoppingCartController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        #endregion

        #region Ctor

        public DelsamiShoppingCartManagementController(ICustomerService customerService,
                                                IPermissionService permissionService,
                                                IShoppingCartService shoppingCartService,
                                                IShoppingCartModelFactory shoppingCartModelFactory) : base(customerService, permissionService, shoppingCartService, shoppingCartModelFactory)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> CurrentCarts()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return AccessDeniedView();

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartSearchModelAsync(new ShoppingCartSearchModel());

            return View("~/Plugins/Delsami.Misc.ShoppingCartManagement/Areas/Admin/Views/DelsamiCurrentCarts.cshtml", model);
        }

        [HttpPost]
        public override async Task<IActionResult> CurrentCarts(ShoppingCartSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCurrentCarts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _shoppingCartModelFactory.PrepareShoppingCartListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
    }
}
