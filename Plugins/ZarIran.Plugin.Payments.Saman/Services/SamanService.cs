using ZarIran.Plugin.Misc.Core.Services.ApiCaller;
using ZarIran.Plugin.Payment.Core.Services;
using ZarIran.Plugin.Payments.Saman.Models.Saman;
using Nop.Core;
using Nop.Services.Logging;
using System;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Saman.Services
{
    public class SamanService : ISamanService
    {
        #region Fields

        private readonly SamanSettings _SamanBPGSettings;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentApiCaller _paymentApiCaller;

        #endregion

        #region Ctor

        public SamanService(SamanSettings SamanBPGSettings,
                            ILogger logger,
                            IPaymentApiCaller paymentApiCaller,
                            IWebHelper webHelper)
        {
            _SamanBPGSettings = SamanBPGSettings;
            _logger = logger;
            _webHelper = webHelper;
            _paymentApiCaller = paymentApiCaller;
        }

        #endregion

        #region Methods
        public async Task<(bool CallIsSuccess, TokenResult tokenResult)> GetToken(int orderId, Payment.Core.Domain.Payment payment, string customerPhone)
        {

            try
            {
                var url = _SamanBPGSettings.BaseUrl + SamanPaymentEndpoints.GetToken;

                var currentStoreUrl = _webHelper.GetStoreLocation();
                if (currentStoreUrl.EndsWith("/"))
                    currentStoreUrl = currentStoreUrl.Substring(0, currentStoreUrl.Length - 1);

                var tokenRequest = new SepTransaction
                {
                    action = "token",
                    Amount = (long)payment.Amount,
                    CellNumber = customerPhone,
                    RedirectUrl = currentStoreUrl + $"/Saman/Verify?paymentId={payment.Id}",
                    ResNum = payment.Id.ToString(),
                    TerminalId = _SamanBPGSettings.TerminalNumber
                };

                var result = await _paymentApiCaller.PostAsync<SepTransaction, TokenResult>(url, tokenRequest);

                return (result.IsSuccess, result.Data);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return (false, null);
            }
        }

        public async Task<(bool CallIsSuccess, VerifyTransactionResult VerifyResponse)> VerifyAsync(VerifyTransactionRequest verifyRequest)
        {
            try
            {
                var url = _SamanBPGSettings.BaseUrl + SamanPaymentEndpoints.Verify;

                var result = await _paymentApiCaller.PostAsync<VerifyTransactionRequest, VerifyTransactionResult>(
                    url,
                    verifyRequest);

                return (result.IsSuccess, result.Data);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return (false, null);
            }
        }


        #endregion

    }
}
