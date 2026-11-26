using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IDelsamiPaymentService : IPaymentService
    {
        Task<decimal> CalculatePaymentMethodDiscountAsync(IList<ShoppingCartItem> cart, string paymentMethodSystemName);
    }
}
