using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Payment.Core.Controllers;
using ZarIran.Plugin.Payment.Core.Enums;
using ZarIran.Plugin.Payment.Core.Models;
using ZarIran.Plugin.Payment.Core.Services;
using ZarIran.Plugin.Payments.Saman.Models.Saman;
using ZarIran.Plugin.Payments.Saman.Services;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Saman.Controllers;

public class SamanController : BaseNopPartsPaymentController<SamanCallbackResult>
{
    #region Fields

    private readonly ISamanService _SamanService;

    protected override PaymentGateway paymentGateway => PaymentGateway.Saman;

    #endregion

    #region Ctor

    public SamanController(ISamanService SamanService,
                          IOrderService orderService,
                          ILocalizationService localizationService,
                          INotificationService notificationService,
                          IPaymentService paymentService,
                          IEventPublisher eventPublisher,
                          ILogger logger,
                          SamanSettings samanSettings,
                          IOrderProcessingService orderProcessingService,
                          CoreSettings coreSetttings) : base(paymentService,
                                                                              orderService,
                                                                              orderProcessingService,
                                                                              logger,
                                                                              notificationService,
                                                                              localizationService,
                                                                              eventPublisher,
                                                                              coreSetttings)
    {
        _SamanService = SamanService;
    }

    #endregion

    #region Methods

    protected override async Task<VerifyingResult> VerifyPayment(Payment.Core.Domain.Payment payment, Order order, SamanCallbackResult model)
    {
        if (model is null)
            return GenerateFailedResult(ErrorCode.GatewayError);

        await _logger.InformationAsync("Saman Pay Result:" + JsonSerializer.Serialize(model));

        var paymentIsSuccessful = model.Status == "2" && model.State == "OK" && model.Amount == payment.Amount.ToString("0");

        var payModel = new PayResultModel()
        {
            TrackId = model.TraceNo,
            TeminalNo = model.TerminalId,
            ResultCode = model.State,
            ReferenceNumber = model.RefNum,
            MaskedCardNumber = model.SecurePan,
            Amount = model.Amount,
            AdditionalData = $"RRN: {model.Rrn} - ResNum: {model.ResNum} - Status: {model.Status}"
        };

        if (!paymentIsSuccessful)
            if (model.State.Equals("CanceledByUser", StringComparison.InvariantCultureIgnoreCase))
                return GenerateFailedResult(ErrorCode.CanceledByUser, payModel);
            else
                return GenerateFailedResult(ErrorCode.GatewayError, payModel);

        try
        {
            VerifyTransactionRequest verifyRequest = new()
            {
                RefNum = model.RefNum,
                TerminalNumber = long.Parse(model.TerminalId),
            };

            await _logger.InformationAsync("Saman Verify Request:" + JsonSerializer.Serialize(verifyRequest));

            var result = await _SamanService.VerifyAsync(verifyRequest);

            await _logger.InformationAsync("Saman Verify Response:" + JsonSerializer.Serialize(result));

            if (!result.CallIsSuccess || result.VerifyResponse == null)
                return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyTransactionResult());



            if (result.VerifyResponse.Success && result.VerifyResponse.ResultCode == 0)
                return GenerateSuccessResult(payModel, result.VerifyResponse, false);


            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: result.VerifyResponse);

        }
        catch (Exception ex)
        {

            await _logger.ErrorAsync($"Saman verify has error", ex);
            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyTransactionResult());

        }

    }

    #endregion

}
