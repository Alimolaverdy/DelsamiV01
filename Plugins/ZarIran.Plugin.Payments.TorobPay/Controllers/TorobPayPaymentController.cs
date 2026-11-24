using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Payments.TorobPay.Services;
using Nop.Services.Orders;
using System;
using System.Threading.Tasks;
using Nop.Services.Logging;
using ZarIran.Plugin.Payment.Core.Services;
using ZarIran.Plugin.Payment.Core.Enums;
using Nop.Core.Events;
using Nop.Services.Messages;
using Nop.Services.Localization;
using ZarIran.Plugin.Payment.Core.Controllers;
using ZarIran.Plugin.Payment.Core.Models;
using Nop.Core.Domain.Orders;
using static ZarIran.Plugin.Payments.TorobPay.Services.Dtos.PaymentDto;
using ZarIran.Plugin.Payments.TorobPay.Services.Dtos;
using ZarIran.Plugin.Misc.Core;


namespace ZarIran.Plugin.Payments.TorobPay.Controllers;

public class TorobPayPaymentController : BaseNopPartsPaymentController<TorobPayResultModel>
{
    #region Fields

    private readonly ITorobPayApiService _apiService;

    protected override PaymentGateway paymentGateway => PaymentGateway.Torob;

    #endregion

    #region Ctor

    public TorobPayPaymentController(ITorobPayApiService apiService,
                                     IOrderService orderService,
                                     ILogger logger,
                                     IPaymentService paymentService,
                                     IEventPublisher eventPublisher,
                                     IOrderProcessingService orderProcessingService,
                                     INotificationService notificationService,
                                     ILocalizationService localizationService,
                                     CoreSettings coreSettings) : base(paymentService,
                                                                                          orderService,
                                                                                          orderProcessingService,
                                                                                          logger,
                                                                                          notificationService,
                                                                                          localizationService,
                                                                                          eventPublisher,
                                                                                          coreSettings)
    {
        _apiService = apiService;

    }


    #endregion

    #region Methods
    [HttpPost]
    protected override async Task<VerifyingResult> VerifyPayment(Payment.Core.Domain.Payment payment, Order order, TorobPayResultModel model = null)
    {
        var paymentToken = order.AuthorizationTransactionCode;

        try
        {
            var response = await _apiService.GetStatusAsync(paymentToken);

            var payModel = new PayResultModel()
            {
                ReferenceNumber = response.transactionId,
                TrackId = response.transactionId,
            };

            if (response.status == "REVERT")
                return GenerateFailedResult(ErrorCode.CanceledByUser, payModel);

            var verify = await _apiService.VerifyAsync(paymentToken);
            await _logger.InformationAsync($"TorobPay Verify OK. txnId={verify.transactionId}");

            var settle = await _apiService.SettleAsync(paymentToken);
            await _logger.InformationAsync($"TorobPay Settle OK. txnId={settle.transactionId}");

            payModel.ResultCode = "0";

            return GenerateSuccessResult(payModel, verify, true, settle);

        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("TorobPay Return error (verify/settle)", ex);

            try
            {
                var response = await _apiService.GetStatusAsync(paymentToken);

                if (!string.Equals(response.status, "SETTLE", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(response.status, "VERIFY", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(response.status, "REVERT", StringComparison.OrdinalIgnoreCase))
                {
                    await _apiService.RevertAsync(paymentToken);
                    await _logger.InformationAsync($"TorobPay Revert done for token={paymentToken}");
                }
                else
                {
                    await _logger.WarningAsync($"TorobPay skip revert: status={response.status}");
                }
            }
            catch (Exception revertEx)
            {
                await _logger.ErrorAsync("TorobPay Revert failed", revertEx);
            }

            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResponse());
        }
    }
}


// Cancel-Update-Status 
//    [HttpPost]
//    public async Task<IActionResult> AdminCancel(string paymentToken)
//    {
//        try
//        {
//            await _apiService.CancelAsync(paymentToken);
//            await _logger.InformationAsync($"TorobPay AdminCancel success: {paymentToken}");
//            return Json(new { success = true, message = "لغو با موفقیت انجام شد." });
//        }
//        catch (Exception ex)
//        {
//            await _logger.ErrorAsync("TorobPay AdminCancel error", ex);
//            return Json(new { success = false, message = "خطا در لغو سفارش." });
//        }
//    }

//    [HttpPost]
//    public async Task<IActionResult> AdminStatus(string paymentToken)
//    {
//        try
//        {
//            var result = await _apiService.StatusAsync(paymentToken);
//            return Json(new { success = true, status = result.status, amount = result.amount, transactionId = result.transactionId });
//        }
//        catch (Exception ex)
//        {
//            await _logger.ErrorAsync("TorobPay AdminStatus error", ex);
//            return Json(new { success = false, message = "خطا در دریافت وضعیت سفارش." });
//        }
//    }

//    [HttpPost]
//    public async Task<IActionResult> AdminUpdate(string paymentToken, int amount)
//    {
//        try
//        {
//            var req = new UpdateRequest
//            {
//                paymentToken = paymentToken,
//                amount = amount,
//                paymentMethodTypeDto = "ONLINE_CREDIT",
//                cartList = new List<Cart>()
//            };
//            await _apiService.UpdateAsync(req);
//            return Json(new { success = true, message = "به‌روزرسانی انجام شد." });
//        }
//        catch (Exception ex)
//        {
//            await _logger.ErrorAsync("TorobPay AdminUpdate error", ex);
//            return Json(new { success = false, message = "خطا در به‌روزرسانی سفارش." });
//        }
//    }
//}
#endregion
