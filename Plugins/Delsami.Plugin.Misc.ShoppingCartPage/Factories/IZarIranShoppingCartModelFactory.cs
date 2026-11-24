using ZarIran.Plugin.Misc.ShoppingCartPage.Models;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShoppingCartPage.Factories
{
    public interface IZarIranShoppingCartModelFactory
    {
        Task<ZarIranShoppingCartModel> PrepareShoppingCartModelAsync(ZarIranShoppingCartModel model,
                                                                                           IList<ShoppingCartItem> cart, bool isEditable = true,
                                                                                           bool validateCheckoutAttributes = false,
                                                                                           bool prepareAndDisplayOrderReviewData = false);
    }
}
