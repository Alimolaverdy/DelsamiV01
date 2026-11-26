using Delsami.Plugin.Misc.ShoppingCartPage.Models;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartPage.Factories
{
    public interface IDelsamiShoppingCartModelFactory
    {
        Task<DelsamiShoppingCartModel> PrepareShoppingCartModelAsync(DelsamiShoppingCartModel model,
                                                                                           IList<ShoppingCartItem> cart, bool isEditable = true,
                                                                                           bool validateCheckoutAttributes = false,
                                                                                           bool prepareAndDisplayOrderReviewData = false);
    }
}
