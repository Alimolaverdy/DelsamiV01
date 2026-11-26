using Delsami.Plugin.Payments.TorobPay.Services.Dtos;
using System.Threading.Tasks;
using static Delsami.Plugin.Payments.TorobPay.Services.Dtos.PaymentDto;

namespace Delsami.Plugin.Payments.TorobPay.Services
{
    public interface ITorobPayApiService
    {
        Task<EligibleResponse> CheckEligibilityAsync(decimal amount);
        Task<PaymentTokenResponse> CreatePaymentTokenAsync(PaymentTokenRequest request);
        Task<VerifyResponse> VerifyAsync(string token);
        Task<SettleResponse> SettleAsync(string token);
        Task CancelAsync(string token);
        Task RevertAsync(string token);
        Task<StatusResponse> GetStatusAsync(string token);
        Task UpdateAsync(UpdateRequest request);

    }
}
