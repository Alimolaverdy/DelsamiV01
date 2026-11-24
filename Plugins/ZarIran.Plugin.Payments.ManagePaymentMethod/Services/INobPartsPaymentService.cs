using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IZarIranPaymentService : IPaymentService
    {
        Task<decimal> CalculatePaymentMethodDiscountAsync(IList<ShoppingCartItem> cart, string paymentMethodSystemName);
    }
}
