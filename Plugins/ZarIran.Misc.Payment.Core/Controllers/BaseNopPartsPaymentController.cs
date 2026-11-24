using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Events;
using ZarIran.Plugin.Payment.Core.Enums;
using ZarIran.Plugin.Payment.Core.Models;
using ZarIran.Plugin.Payment.Core.Services;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payment.Core.Controllers;

public abstract class BaseNopPartsPaymentController<T> : BasePaymentController where T : BasePaymentResponseModel
{
    #region Fields

    protected readonly IPaymentService _paymentService;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly INotificationService _notificationService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IEventPublisher _eventPublisher;
    private readonly CoreSettings _coreSettings;

    private VerifyingResult _verifyingResult;
    protected abstract PaymentGateway paymentGateway { get; }

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    #endregion

    #region Ctor

    protected BaseNopPartsPaymentController(IPaymentService paymentService,
                                            IOrderService orderService,
                                            IOrderProcessingService orderProcessingService,
                                            ILogger logger,
                                            INotificationService notificationService,
                                            ILocalizationService localizationService,
                                            IEventPublisher eventPublisher,
                                            CoreSettings coreSettings)
    {
        _eventPublisher = eventPublisher;
        _paymentService = paymentService;
        _orderService = orderService;
        _orderProcessingService = orderProcessingService;
        _logger = logger;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _coreSettings = coreSettings;
    }

    #endregion

    #region BaseMethods 

    public async Task<IActionResult> VerifyGet(int paymentId, T model)
    {
        return await Verify(paymentId, model);
    }

    [HttpPost]
    public async Task<IActionResult> Verify(int paymentId, T model)
    {
        if (paymentId == 0)
            return RedirectToRoute("Checkout/PaymentError", new { ErrorCode.PaymentNotFound });

        var semaphore = _locks.GetOrAdd($"Payment.{paymentGateway.ToString()}.Verify.{paymentId}", _ => new SemaphoreSlim(1, 1));

        IActionResult actionResult = null;

        await semaphore.WaitAsync(60000);

        try
        {
            _verifyingResult = await PaymentValidation(paymentId);

            if (!_verifyingResult.IsSuccess)
                return _verifyingResult.ActionResult;

            var verifyingResult = await VerifyPayment(_verifyingResult.Payment, _verifyingResult.Order, model);

            if (verifyingResult.IsSuccess)
                actionResult = await SuccessPath(verifyingResult);
            else
                actionResult = await FailedPath(verifyingResult);


            if (actionResult is null)
                actionResult = Redirect("/");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"exception error in {paymentGateway.ToString()}:ex.Message", ex);

            return Redirect("/");
        }
        finally
        {
            semaphore.Release();
        }

        return actionResult;
    }

    protected virtual async Task<IActionResult> SuccessPath(VerifyingResult verifyResult)
    {
        var note = $"پرداخت با موفقیت انجام شد";

        if (verifyResult.PayResult != null)
            note += $" - اطلاعات پرداخت: {verifyResult.PayResult.ToString()}";

        if (verifyResult.VerifyResult != null)
            note += $" - اطلاعات تایید تراکنش: {verifyResult.VerifyResult.ToString()}";

        if (verifyResult.ConfirmResult != null)
            note += $" - اطلاعات سومین قدم تراکنش: {verifyResult.ConfirmResult.ToString()}";

        var orderNote = new OrderNote
        {
            Note = note,
            CreatedOnUtc = DateTime.UtcNow,
            DisplayToCustomer = false,
            OrderId = verifyResult.Order.Id
        };

        verifyResult.Order.CaptureTransactionId = verifyResult.PayResult.ReferenceNumber;

        await _orderService.UpdateOrderAsync(verifyResult.Order);

        verifyResult.Payment.ReferenceNumber = verifyResult.PayResult.ReferenceNumber;
        verifyResult.Payment.TrackId = verifyResult.PayResult.TrackId;
        verifyResult.Payment.ResultCode = verifyResult.PayResult.ResultCode;
        verifyResult.Payment.Status = verifyResult.IsConfirm ? PaymentStatus.Confirmed : PaymentStatus.Verified;

        if (verifyResult.IsConfirm)
            verifyResult.Payment.ConfirmDateTime = DateTime.UtcNow;
        else
            verifyResult.Payment.VerifyDateTime = DateTime.UtcNow;

        verifyResult.Payment.Completed = true;
        verifyResult.Payment.MaskedCardNumber = verifyResult.PayResult.MaskedCardNumber;
        verifyResult.Payment.HashedCardNumber = verifyResult.PayResult.HashedCardNumber;
        verifyResult.Payment.AdditionalData = verifyResult.PayResult.AdditionalData;

        await _paymentService.UpdateAsync(verifyResult.Payment);

        await _orderService.InsertOrderNoteAsync(orderNote);

        await _orderProcessingService.MarkOrderAsPaidAsync(verifyResult.Order);

        await _eventPublisher.PublishAsync(new OrderProcessingStartEvent(verifyResult.Order));

        return RedirectToAction("completed", "checkout", new { orderId = verifyResult.Order.Id });
    }

    protected virtual async Task<IActionResult> FailedPath(VerifyingResult verifyResult)
    {
        string note = string.Empty;

        if (verifyResult.PayResult != null)
            note = $"پرداخت از درگاه  {paymentGateway.ToString()} با خطا مواجه شد. اطلاعات پرداخت : {verifyResult.PayResult.ToString()}";

        if (verifyResult.VerifyResult != null)
            note = $"تایید پرداخت از درگاه {paymentGateway.ToString()} با خطا مواجه شد. اطلاعات تایید پرداخت: {verifyResult.VerifyResult.ToString()}";

        if (verifyResult.ConfirmResult != null)
            note = $"سومین قدم از درگاه {paymentGateway.ToString()} با خطا مواجه شد. اطلاعات سومین قدم: {verifyResult.ConfirmResult.ToString()}";

        if (!string.IsNullOrEmpty(note))
        {

            var orderNote = new OrderNote
            {
                Note = note,
                CreatedOnUtc = DateTime.UtcNow,
                DisplayToCustomer = false,
                OrderId = verifyResult.Order.Id
            };

            await _orderService.InsertOrderNoteAsync(orderNote);
        }

        verifyResult.Payment.Status = PaymentStatus.Failed;
        verifyResult.Payment.CanceledByUser = verifyResult.CanceledByUser;
        await _paymentService.UpdateAsync(verifyResult.Payment);

        if (verifyResult.ActionResult is null)
            return base.RedirectToRoute("RefillBasket");

        return verifyResult.ActionResult;
    }

    protected virtual async Task<VerifyingResult> PaymentValidation(int paymentId)
    {
        var payment = await _paymentService.GetAsync(paymentId);

        if (payment == null)
        {
            await _logger.ErrorAsync($"{paymentGateway.ToString()} - payment not found. payementId={paymentId}");
            return new()
            {
                ActionResult = RedirectToRoute("Checkout/PaymentError"),
                IsSuccess = false,
            };
        }

        if (payment.CanceledByUser)
            return new()
            {
                IsSuccess = false,
                ActionResult = RedirectToRoute("RefillBasket")
            };

        var order = await _orderService.GetOrderByIdAsync(payment.OrderId);
        if (order == null)
        {
            await _logger.ErrorAsync($"{paymentGateway.ToString()} - order not found. orderId={payment.OrderId}");
            return new()
            {
                ActionResult = RedirectToRoute("Checkout/PaymentError"),
                IsSuccess = false,
            };
        }

        if (order.OrderStatus == OrderStatus.Processing || order.OrderStatus == OrderStatus.Complete)
            return new()
            {
                ActionResult = RedirectToAction("completed", "checkout", new { orderId = payment.OrderId }),
                IsSuccess = false
            };

        if (!_orderProcessingService.CanMarkOrderAsPaid(order))
            return new()
            {
                ActionResult = RedirectToRoute("Checkout/PaymentError", new { order.Id }),
                IsSuccess = false,
            };

        return new() { IsSuccess = true, Order = order, Payment = payment };
    }

    protected VerifyingResult GenerateFailedResult(ErrorCode errorCode,
                                                   PayResultModel payModel = null,
                                                   BasePaymentResponseModel verifyModel = null,
                                                   BasePaymentResponseModel thirdStep = null)
    {

        return new VerifyingResult
        {
            IsSuccess = false,
            Payment = _verifyingResult.Payment,
            Order = _verifyingResult.Order,
            VerifyResult = verifyModel,
            ConfirmResult = thirdStep,
            IsConfirm = false,
            PayResult = payModel,
            CanceledByUser = errorCode == ErrorCode.CanceledByUser,
            ActionResult = errorCode == ErrorCode.CanceledByUser ?
            RedirectToRoute("RefillBasket") :
            RedirectToRoute("Checkout/PaymentError", new { orderId = _verifyingResult.Order.Id })
        };
    }

    protected VerifyingResult GenerateSuccessResult(PayResultModel payResult,
                                                    BasePaymentResponseModel verifyResult,
                                                    bool isConfirm,
                                                    BasePaymentResponseModel confirmResult = null)
    {
        return new()
        {
            IsSuccess = true,
            Payment = _verifyingResult.Payment,
            Order = _verifyingResult.Order,
            VerifyResult = verifyResult,
            ConfirmResult = confirmResult,
            IsConfirm = isConfirm,
            PayResult = payResult,
            ActionResult = null,

        };
    }
    #endregion

    #region Abstracts

    protected abstract Task<VerifyingResult> VerifyPayment(Domain.Payment payment, Order order, T model = default);

    #endregion

}

