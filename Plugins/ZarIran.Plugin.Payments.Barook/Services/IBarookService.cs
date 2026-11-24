using ZarIran.Plugin.Payments.Barook.Models.Barook;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Barook.Services;

public interface IBarookService
{
    Task<(bool CallIsSuccess, StartPaymentResponse tokenResult)> GetToken(Payment.Core.Domain.Payment payment, Customer  customer);

    Task<(bool CallIsSuccess, VerifyPaymentResponse VerifyResponse)> VerifyAsync(VerifyPaymentRequest verifyRequest);

}
