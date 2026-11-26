using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.PaymentMethodDiscounts;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories
{
    public interface IPaymentMethodDiscountModelFactory
    {
        Task<PaymentMethodDiscountSearchModel> PreparePaymentMethodDiscountSearchModelAsync(PaymentMethodDiscountSearchModel searchModel);

        Task<PaymentMethodDiscountModel> PreparePaymentMethodDiscountModelAsync(PaymentMethodDiscountModel model, PaymentMethodDiscount paymentMethodDiscount);

        Task<PaymentMethodDiscountListModel> PreparePaymentMethodDiscountListModelAsync(PaymentMethodDiscountSearchModel searchModel);
    }
}
