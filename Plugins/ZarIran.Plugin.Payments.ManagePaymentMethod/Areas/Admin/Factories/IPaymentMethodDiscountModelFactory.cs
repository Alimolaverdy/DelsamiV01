using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.PaymentMethodDiscounts;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories
{
    public interface IPaymentMethodDiscountModelFactory
    {
        Task<PaymentMethodDiscountSearchModel> PreparePaymentMethodDiscountSearchModelAsync(PaymentMethodDiscountSearchModel searchModel);

        Task<PaymentMethodDiscountModel> PreparePaymentMethodDiscountModelAsync(PaymentMethodDiscountModel model, PaymentMethodDiscount paymentMethodDiscount);

        Task<PaymentMethodDiscountListModel> PreparePaymentMethodDiscountListModelAsync(PaymentMethodDiscountSearchModel searchModel);
    }
}
