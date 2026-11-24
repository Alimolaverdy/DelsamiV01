using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IPaymentMethodRewardPointService
    {
        Task InsertPaymentMethodRewardPointAsync(PaymentMethodRewardPoint paymentMethodRewardPoint);
        Task UpdateAsync(PaymentMethodRewardPoint paymentMethodRewardPoint);
        Task<PaymentMethodRewardPoint> GetPaymentMethodRewardPointAsync(int orderId);

    }
}
