using Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Factories
{
    public interface IFulfillmentDecisionFactory
    {
        Task<FulfillmentDecisionRequestDto> PrepareFulfillmentDecisionRequestAsync(IList<ShoppingCartItem> cartItems, Address shippingAddress);
    }
}
