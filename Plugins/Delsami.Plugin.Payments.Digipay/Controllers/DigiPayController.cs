using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payment.Core.Services;
using Delsami.Plugin.Payments.Digipay.Factories;
using Delsami.Plugin.Payments.Digipay.Models.DigiPays;
using Delsami.Plugin.Payments.Digipay.Services;
using Nop.Core;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Delsami.Plugin.Payment.Core.Controllers;
using Delsami.Plugin.Payment.Core.Models;
using Nop.Core.Domain.Orders;
using System.Text.Json;
using Delsami.Plugin.Misc.Core;

namespace Delsami.Plugin.Payments.Digipay.Controllers;

public class DigiPayController : BaseDelsamiPaymentController<VerifyResultModel>
{
    #region Fields

    private readonly IDigiPayService _digipayService;
    private readonly IWorkContext _workContext;
    private readonly IBPGModelFactory _bpgModelFactory;
    private readonly IProductService _productService;

    protected override Payment.Core.Enums.PaymentGateway paymentGateway => Payment.Core.Enums.PaymentGateway.Digipay;

    #endregion

    #region Ctor

    public DigiPayController(IDigiPayService digipayService,
                                IGenericAttributeService genericAttributeService,
                                IWorkContext workContext,
                                IOrderService orderService,
                                IBPGModelFactory bpgModelFactory,
                                ILocalizationService localizationService,
                                INotificationService notificationService,
                                IProductService productService,
                                IPaymentService paymentService,
                                IEventPublisher eventPublisher,
                                ILogger logger,
                                IOrderProcessingService orderProcessingService,
                                CoreSettings coreSettings) : base(paymentService,
                                                                                     orderService,
                                                                                     orderProcessingService,
                                                                                     logger,
                                                                                     notificationService,
                                                                                     localizationService,
                                                                                     eventPublisher,
                                                                                     coreSettings)
    {
        _digipayService = digipayService;
        _workContext = workContext;
        _bpgModelFactory = bpgModelFactory;
        _productService = productService;
    }


    #endregion

    protected override async Task<VerifyingResult> VerifyPayment(Payment.Core.Domain.Payment payment, Order order, VerifyResultModel model = null)
    {
        if (model is null)
            return GenerateFailedResult(ErrorCode.GatewayError);

        await _logger.InformationAsync("Digipay Pay Result:" + JsonSerializer.Serialize(model));

        PayResultModel payModel = new()
        {
            Amount = model.amount.ToString(),
            ReferenceNumber = model.rrn,
            TrackId = model.trackingCode,
            ResultCode = model.result.ToString(),
            MaskedCardNumber = "",
            TeminalNo = "",
            AdditionalData = $"Type: {model.type} - isCredit: {model.isCredit} - providerId: {model.providerId} - psp: {model.psp}",
        };

        var paymentIsSuccessful = model.result == PaymentResult.SUCCESS && Convert.ToInt64(order.OrderTotal) == model.amount;

        if (!paymentIsSuccessful)
        {
            if (model.result == PaymentResult.CANCEL || model.result == PaymentResult.FAILURE)
                return GenerateFailedResult(ErrorCode.CanceledByUser, payModel);
            else
                return GenerateFailedResult(ErrorCode.GatewayError, payModel);
        }

        try
        {
            var tokenResponse = await _digipayService.GetTokenAsync();

            var verifyRequest = _bpgModelFactory.PrepareVerifyRequest(payment.Id, model.trackingCode);

            await _logger.InformationAsync("Digipay Verify Request:" + JsonSerializer.Serialize(new { verifyRequest, tokenResponse.TokenResult.access_token, model.type }));

            var verifyResponse = await _digipayService.VerifyAsync(verifyRequest, tokenResponse.TokenResult.access_token, model.type);

            await _logger.InformationAsync("Digipay Verify Response:" + JsonSerializer.Serialize(verifyResponse));


            if (!verifyResponse.CallIsSuccess || verifyResponse.VerifyResponse == null)
                return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResponseModel());

            if (verifyResponse.VerifyResponse.result?.status != 0)
                return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: verifyResponse.VerifyResponse);

            //type=5 CREDIT
            //type=13 BNPL
            //type=11 Wallet
            if (model.type == 11)
                return GenerateSuccessResult(payModel, verifyResponse.VerifyResponse, true);


            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
            var products = await _productService.GetProductsByIdsAsync(orderItems.Select(s => s.ProductId).ToArray());

            var productNames = products.Select(s => s.Name).ToList();
            var deliverRequest = _bpgModelFactory.PrepareDeliverRequest(order.Id, model.trackingCode, productNames);

            await _logger.InformationAsync("Digipay Deliver Request:" + JsonSerializer.Serialize(new { deliverRequest, tokenResponse.TokenResult.access_token, model.type }));

            var deliverResponse = await _digipayService.DeliverAsync(deliverRequest, tokenResponse.TokenResult.access_token, model.type);

            await _logger.InformationAsync("Digipay Deliver Response:" + JsonSerializer.Serialize(deliverResponse));

            if (!deliverResponse.CallIsSuccess || deliverResponse.DeliverResponse == null)
                return GenerateFailedResult(ErrorCode.GatewayError, thirdStep: new DeliverResponseModel());


            if (deliverResponse.DeliverResponse.result?.status == 0)
                return GenerateSuccessResult(payModel, verifyResponse.VerifyResponse, true, deliverResponse.DeliverResponse);

            return GenerateFailedResult(ErrorCode.GatewayError, thirdStep: deliverResponse.DeliverResponse);

        }
        catch (Exception)
        {
            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResponseModel());
        }

    }
}
