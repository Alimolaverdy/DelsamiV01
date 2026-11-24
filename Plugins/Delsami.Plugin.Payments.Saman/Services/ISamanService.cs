using ZarIran.Plugin.Payments.Saman.Models.Saman;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Saman.Services;

public interface ISamanService
{
    Task<(bool CallIsSuccess, TokenResult tokenResult)> GetToken(int orderId, Payment.Core.Domain.Payment payment, string customerPhone);

    Task<(bool CallIsSuccess, VerifyTransactionResult VerifyResponse)> VerifyAsync(VerifyTransactionRequest verifyRequest);


}
