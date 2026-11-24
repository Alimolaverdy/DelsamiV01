using Microsoft.IdentityModel.Tokens;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public class PaymentMethodRewardPointService : IPaymentMethodRewardPointService
    {
        private readonly IRepository<PaymentMethodRewardPoint> _paymentMethodRewardPoint;

        public PaymentMethodRewardPointService(IRepository<PaymentMethodRewardPoint> paymentMethodRewardPoint)
        {
            _paymentMethodRewardPoint = paymentMethodRewardPoint;
        }


        public async Task<PaymentMethodRewardPoint> GetPaymentMethodRewardPointAsync(int orderId)
        {
            if (orderId == 0) {return null;}

            var paymentMethodRewardPoint = await _paymentMethodRewardPoint.Table.FirstOrDefaultAsync(f => f.OrderId == orderId);
            return paymentMethodRewardPoint;
        }

        public async Task InsertPaymentMethodRewardPointAsync(PaymentMethodRewardPoint paymentMethodRewardPoint)
        {
            await _paymentMethodRewardPoint.InsertAsync(paymentMethodRewardPoint);
        }

        public async Task UpdateAsync(PaymentMethodRewardPoint paymentMethodRewardPoint)
        {
            if (paymentMethodRewardPoint == null)
                throw new ArgumentNullException(nameof(paymentMethodRewardPoint));

            await _paymentMethodRewardPoint.UpdateAsync(paymentMethodRewardPoint);
        }
    }
}
