using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using static Nop.Web.Models.Order.CustomerOrderListModel;

namespace ZarIran.Plugin.Misc.ShoppingCartPage.Models
{
    public record ZarIranShoppingCartModel : ShoppingCartModel
    {
        public ZarIranShoppingCartModel()
        {
            PendingOrders = new List<OrderDetailsModel>();
        }

        public IList<OrderDetailsModel> PendingOrders { get; set; }

    }
}
