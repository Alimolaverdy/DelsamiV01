using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Payment.Core.Controllers;
using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payment.Core.Models;
using Delsami.Plugin.Payment.Core.Services;
using Delsami.Plugin.Payments.Barook.Models.Barook;
using Delsami.Plugin.Payments.Barook.Services;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Barook.Controllers;

public class BarookController : BaseDelsamiPaymentController<BarookPaymentCallback>
{
    #region Fields

    private readonly IBarookService _BarookService;
    private readonly BarookSettings _barookSettings;

    protected override PaymentGateway paymentGateway => PaymentGateway.Barook;

    #endregion

    #region Ctor

    public BarookController(IBarookService BarookService,
                          IOrderService orderService,
                          ILocalizationService localizationService,
                          INotificationService notificationService,
                          IPaymentService paymentService,
                          IEventPublisher eventPublisher,
                          ILogger logger,
                          BarookSettings BarookSettings,
                          IOrderProcessingService orderProcessingService,
                          BarookSettings barookSettings,
                          CoreSettings coreSetttings) : base(paymentService,
                                                                  orderService,
                                                                  orderProcessingService,
                                                                  logger,
                                                                  notificationService,
                                                                  localizationService,
                                                                  eventPublisher,
                                                                  coreSetttings)
    {
        _BarookService = BarookService;
        _barookSettings = barookSettings;
    }

    #endregion

    #region Methods

    protected override async Task<VerifyingResult> VerifyPayment(Payment.Core.Domain.Payment payment, Order order, BarookPaymentCallback model)
    {
        if (model is null)
            return GenerateFailedResult(ErrorCode.GatewayError);

        await _logger.InformationAsync("Barook Pay Result:" + JsonSerializer.Serialize(model));

        var paymentIsSuccessful = model.Status == TransactionStatusEnum.COMPLETED;

        var payModel = new PayResultModel()
        {
            ResultCode = model.Status.ToString(),
            MaskedCardNumber = model.CardNumber,
            ReferenceNumber = model.Token,
        };

        if (!paymentIsSuccessful)
            if (model.Status == TransactionStatusEnum.CANCELED)
                return GenerateFailedResult(ErrorCode.CanceledByUser, payModel);
            else
                return GenerateFailedResult(ErrorCode.GatewayError, payModel);

        try
        {
            VerifyPaymentRequest verifyRequest = new()
            {
                ExternalCode = payment.Id.ToString(),
                Password = _barookSettings.TerminalPassword,
                TerminalCode = _barookSettings.TerminalCode,
                Token = order.AuthorizationTransactionCode,
            };

            await _logger.InformationAsync("Barook Verify Request:" + JsonSerializer.Serialize(verifyRequest));

            var result = await _BarookService.VerifyAsync(verifyRequest);

            await _logger.InformationAsync("Barook Verify Response:" + JsonSerializer.Serialize(result));

            if (!result.CallIsSuccess || result.VerifyResponse == null)
                return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyPaymentResponse());



            if (result.VerifyResponse.Status == TransactionStatusEnum.COMPLETED)
            {
                payment.ReferenceNumber = result.VerifyResponse.ReferenceNumber;
                payment.TeminalNo = result.VerifyResponse.terminalNumber;

                return GenerateSuccessResult(payModel, result.VerifyResponse, false);
            }


            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: result.VerifyResponse);

        }
        catch (Exception ex)
        {

            await _logger.ErrorAsync($"Barook verify has error", ex);
            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyPaymentResponse());

        }

    }

    #endregion

}
