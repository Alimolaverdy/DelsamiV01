using ZarIran.Plugin.Payment.Core.Areas.Admin.Models;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payment.Core.Areas.Admin.Factories
{
    public interface IPaymentModelFactory
    {
        PaymentSearchModel PreparePaymentSearchModel(PaymentSearchModel searchModel);

        Task<PaymentModel> PreparePaymentModelAsync(PaymentModel model, Domain.Payment payment);

        Task<PaymentListModel> PreparePaymentListModelAsync(PaymentSearchModel searchModel);
    }
}
