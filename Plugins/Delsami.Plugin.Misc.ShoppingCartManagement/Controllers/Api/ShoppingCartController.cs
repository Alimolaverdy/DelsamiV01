using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartManagement.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public ShoppingCartController(
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            IProductService productService)
        {
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _pictureService = pictureService;
            _productService = productService;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> GetCartAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Unauthorized(new { error = "User not logged in" });

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer);

            var items = cart.Select(c =>
            {
                var product = _productService.GetProductByIdAsync(c.ProductId).GetAwaiter().GetResult();

                return new
                {
                    /*product = product,*/
                    productId = product.Id,
                    productName = product.Name,
                    quantity = c.Quantity
                };
            }).ToArray();


            return Ok(new
            {
                customerId = customer.Id,
                items
            });
        }

        #endregion
    }
}
