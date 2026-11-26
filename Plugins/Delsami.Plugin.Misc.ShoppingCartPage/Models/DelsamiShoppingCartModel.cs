using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using static Nop.Web.Models.Order.CustomerOrderListModel;

namespace Delsami.Plugin.Misc.ShoppingCartPage.Models
{
    public record DelsamiShoppingCartModel : ShoppingCartModel
    {
        public DelsamiShoppingCartModel()
        {
            PendingOrders = new List<OrderDetailsModel>();
        }

        public IList<OrderDetailsModel> PendingOrders { get; set; }

    }
}
