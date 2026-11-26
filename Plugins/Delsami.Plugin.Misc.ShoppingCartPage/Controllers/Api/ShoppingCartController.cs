using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Misc.Core.Bases;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartPage.Controllers.Api
{
    public class ShoppingCartController : BaseApiController
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public ShoppingCartController(IShoppingCartService shoppingCartService, IWorkContext workContext, ILocalizationService localizationService)
        {
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _localizationService = localizationService;
        }

        [HttpGet("Api/V1/shoppingCartQuantity")]
        public async Task<IActionResult> GetShoppingCartQuantity()
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync());
            var resource = await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity");

            return Ok(string.Format(resource, cart.Sum(s => s.Quantity)));
        }
    }
}
