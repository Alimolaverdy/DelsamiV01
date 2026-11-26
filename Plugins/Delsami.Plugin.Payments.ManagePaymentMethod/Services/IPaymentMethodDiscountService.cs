using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Core;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IPaymentMethodDiscountService
    {
        Task<IPagedList<PaymentMethodDiscount>> GetPaymentMethodDiscountsAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        Task InsertAsync(PaymentMethodDiscount paymentMethodDiscount);

        Task<PaymentMethodDiscount> GetActiveAsync(string paymentSystemName);

        Task UpdateAsync(PaymentMethodDiscount paymentMethodDiscount);

        Task<PaymentMethodDiscount> GetPaymentMethodDiscountAsync(int id);

        Task EndAsync(PaymentMethodDiscount paymentMethodDiscount);

    }
}
