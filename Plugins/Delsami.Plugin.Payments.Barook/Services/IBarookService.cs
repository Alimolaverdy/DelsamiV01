using Delsami.Plugin.Payments.Barook.Models.Barook;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Barook.Services;

public interface IBarookService
{
    Task<(bool CallIsSuccess, StartPaymentResponse tokenResult)> GetToken(Payment.Core.Domain.Payment payment, Customer  customer);

    Task<(bool CallIsSuccess, VerifyPaymentResponse VerifyResponse)> VerifyAsync(VerifyPaymentRequest verifyRequest);

}
