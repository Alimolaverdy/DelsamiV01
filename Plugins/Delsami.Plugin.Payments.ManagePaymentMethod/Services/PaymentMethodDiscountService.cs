using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Core;
using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public class PaymentMethodDiscountService : IPaymentMethodDiscountService
    {
        #region Fields

        private readonly IRepository<PaymentMethodDiscount> _paymentMethodDiscountRepo;

        #endregion

        #region Ctor

        public PaymentMethodDiscountService(IRepository<PaymentMethodDiscount> paymentMethodDiscountRepo)
        {
            _paymentMethodDiscountRepo = paymentMethodDiscountRepo;
        }

        #endregion

        #region Methods

        public virtual async Task<IPagedList<PaymentMethodDiscount>> GetPaymentMethodDiscountsAsync(int pageIndex = 0,
                                                                                                    int pageSize = int.MaxValue)
        {
            var paymentMethodDiscounts = await _paymentMethodDiscountRepo.GetAllPagedAsync(query =>
            {
                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return paymentMethodDiscounts;
        }

        public async Task InsertAsync(PaymentMethodDiscount paymentMethodDiscount)
        {
            if (paymentMethodDiscount == null)
                throw new ArgumentNullException(nameof(paymentMethodDiscount));

            await _paymentMethodDiscountRepo.InsertAsync(paymentMethodDiscount);
        }

        public async Task<PaymentMethodDiscount> GetActiveAsync(string paymentSystemName)
        {
            if (string.IsNullOrEmpty(paymentSystemName))
                return null;

            var paymentMethodDiscount = await _paymentMethodDiscountRepo.Table.FirstOrDefaultAsync(f => f.PaymentMethodSystemName == paymentSystemName && !f.EndDateTimeOnUtc.HasValue);

            return paymentMethodDiscount;
        }

        public async Task UpdateAsync(PaymentMethodDiscount paymentMethodDiscount)
        {
            if (paymentMethodDiscount == null)
                throw new ArgumentNullException(nameof(paymentMethodDiscount));

            await _paymentMethodDiscountRepo.UpdateAsync(paymentMethodDiscount);
        }

        public async Task<PaymentMethodDiscount> GetPaymentMethodDiscountAsync(int id)
        {
            var paymentMethodDiscount = await _paymentMethodDiscountRepo.Table.FirstOrDefaultAsync(f => f.Id == id);

            return paymentMethodDiscount;
        }


        public async Task EndAsync(PaymentMethodDiscount paymentMethodDiscount)
        {
            if (paymentMethodDiscount == null)
                throw new ArgumentNullException(nameof(paymentMethodDiscount));

            paymentMethodDiscount.EndDateTimeOnUtc = DateTime.UtcNow;

            await _paymentMethodDiscountRepo.UpdateAsync(paymentMethodDiscount);
        }

        #endregion
    }
}
