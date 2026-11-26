using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Payment.Core.Controllers;
using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payment.Core.Models;
using Delsami.Plugin.Payment.Core.Services;
using Delsami.Plugin.Payment.Tejarat.Enums;
using Delsami.Plugin.Payment.Tejarat.Helpers;
using Delsami.Plugin.Payment.Tejarat.Infrastructure;
using Delsami.Plugin.Payment.Tejarat.Models.Payments;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Tejarat.Controllers;

public class TejaratBankController : BaseDelsamiPaymentController<TejaratPayResultModel>
{
    #region Fields

    private readonly TejaratSettings _tejaratSettings;
    private readonly IWebHelper _webHelper;
    private readonly ICustomerService _customerService;
    private readonly IPaymentApiCaller _paymentApiCaller;

    #endregion

    #region Ctor

    public TejaratBankController(IOrderService orderService,
                                 TejaratSettings tejaratSettings,
                                 IPaymentService paymentService,
                                 IWebHelper webHelper,
                                 IPaymentApiCaller paymentApiCaller,
                                 IOrderProcessingService orderProcessingService,
                                 ILogger logger,
                                 INotificationService notificationService,
                                 ILocalizationService localizationService,
                                 ICustomerService customerService,
                                 IEventPublisher eventPublisher,
                                 CoreSettings coreSettings) : base(paymentService,
                                                                                         orderService,
                                                                                         orderProcessingService,
                                                                                         logger,
                                                                                         notificationService,
                                                                                         localizationService,
                                                                                         eventPublisher,
                                                                                         coreSettings
                                                                                         )
    {
        _paymentApiCaller = paymentApiCaller;
        _tejaratSettings = tejaratSettings;
        _webHelper = webHelper;
        _customerService = customerService;
    }

    protected override PaymentGateway paymentGateway => PaymentGateway.Tejarat;

    #endregion

    #region Methods

    public async Task<IActionResult> Pay(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var payment = new Core.Domain.Payment
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                SourceType = SourceType.Order,
                Gateway = PaymentGateway.Tejarat,
                Amount = Convert.ToInt64(order.OrderTotal),
                CreatedDateTime = DateTime.UtcNow,
                Status = PaymentStatus.Initial,
                TeminalNo = _tejaratSettings.TerminalId,
                PaymentType = PaymentType.IPG
            };

            await _paymentService.InsertAsync(payment);


            string paymentId = payment.Id.ToString();
            string requestId = payment.Id.ToString();
            //string paymentId = new Random().Next().ToString();
            //string requestId = new Random().Next().ToString();
            IPGDataModel _iPGData = new IPGDataModel();
            _iPGData.TerminalId = _tejaratSettings.TerminalId;
            _iPGData.AcceptorId = _tejaratSettings.AcceptorId;

            //آدرس برگشت اینجا ست می شود
            _iPGData.RevertURL = $"{_webHelper.GetStoreLocation()}TejaratBank/Verify?paymentId={payment.Id}";
            //_iPGData.RevertURL = "http://localhost:6001/Verifypage/Verify";
            _iPGData.Amount = (long)payment.Amount;
            _iPGData.PaymentId = paymentId;
            _iPGData.RequestId = requestId;

            //the url that must be hard coded
            if (ValidationHelper.MobilePhoneNumberIsValid(customer.Username))
                _iPGData.CmsPreservationId = "98" + customer.Username.Substring(1);

            _iPGData.TransactionType = TransactionType.Purchase.ToString();
            _iPGData.BillInfo = null;
            _iPGData.PassPhrase = _tejaratSettings.PassPhrase;


            // کلید عمومی 
            _iPGData.RsaPublicKey = _tejaratSettings.RsaPublicKey;


            /// call json request 
            var request = CreateJsonRequest.CreateJasonRequest(_iPGData);

            // پاس دادن اطلاعات ورودی به api
            //string jresponse = await HttpRequestHelper.PostAsync(Constant.GetTokenUrl, request);

            //TokenResult jResult = JsonConvert.DeserializeObject<TokenResult>(jresponse);

            var jresponse = await _paymentApiCaller.PostAsync<RequestClass, TokenResult>(Constant.GetTokenUrl, request);
            string errorMessage = string.Empty;
            if (jresponse.IsSuccess && jresponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jResult = jresponse.Data;
                string token = jResult.result?.token;

                if (token != null)
                {
                    payment.UrlId = token;

                    NameValueCollection dataCollection = new NameValueCollection
                {
                    { "tokenIdentity", token }
                };

                    var preparedForm = PreparePOSTFormSaman(Constant.PaymentUrl, dataCollection);

                    payment.Status = PaymentStatus.InProgress;

                    await _paymentService.UpdateAsync(payment);

                    return View("~/Plugins/Delsami.Payment.Tejarat/Views/Pay.cshtml", preparedForm);
                }

                if (jresponse.Data != null)
                {
                    errorMessage = string.Format("result:{0} error:{1}", jResult.responseCode, jresponse.ErrorMessage);

                    switch (jResult.responseCode)
                    {
                        case "901":
                            errorMessage = string.Format(" درخواست نا معتبر است  {0}", jResult.responseCode);
                            break;

                        case "902":
                            errorMessage = string.Format(" پارامترهای اضافی درخواست نامعتبر می باشد  {0}", jResult.responseCode);
                            break;

                        case "903":
                            errorMessage = string.Format(" شناسه پرداخت نامعتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "904":
                            errorMessage = string.Format(" اطلاعات مرتبط با قبض نا معتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "905":
                            errorMessage = string.Format(" شناسه درخواست نامعتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "906":
                            errorMessage = string.Format(" درخواست تاریخ گذشته است {0}", jResult.responseCode);
                            break;

                        case "907":
                            errorMessage = string.Format(" آدرس بازگشت نتیجه پرداخت نامعتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "909":
                            errorMessage = string.Format(" پذیرنده نامعتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "910":
                            errorMessage = string.Format(" پارامترهای مورد انتشار پرداخت تسهیمی تامین نگردیده است {0}", jResult.responseCode);
                            break;

                        case "911":
                            errorMessage = string.Format(" پارامترهای مورد انتشار پرداخت تسهیمی نا معتبر یا دارای اشکال می باشد {0}", jResult.responseCode);
                            break;

                        case "912":
                            errorMessage = string.Format(" تراکنش درخواستی برای پذیرنده فعال نیست {0}", jResult.responseCode);
                            break;

                        case "913":
                            errorMessage = string.Format(" تراکنش تسهیم برای پذیرنده  فعال نیست {0}", jResult.responseCode);
                            break;

                        case "914":
                            errorMessage = string.Format("  آدرس آی پی دریافتی درخواست نا معتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "915":
                            errorMessage = string.Format("   شماره پایانه نامعتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "916":
                            errorMessage = string.Format("   شماره پذیرنده نا معتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "917":
                            errorMessage = string.Format("   نوع تراکنش اعلام شده در خواست نا معتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "918":
                            errorMessage = string.Format("   پذیرنده فعال نیست {0}", jResult.responseCode);
                            break;

                        case "919":
                            errorMessage = string.Format("   مبالغ تسهیمی ارائه شده با توجه به قوانین حاکم بر وضعیت تسهیم پذیرنده ، نا معتبر است {0}", jResult.responseCode);
                            break;

                        case "920":
                            errorMessage = string.Format("   شناسه نشانه نامعتبر می باشد {0}", jResult.responseCode);
                            break;

                        case "921":
                            errorMessage = string.Format("   شناسه نشانه نامعتبر و یا منقضی شده است {0}", jResult.responseCode);
                            break;

                        case "922":
                            errorMessage = string.Format(" نقض امنیت درخواست  {0}", jResult.responseCode);
                            break;

                        case "923":
                            errorMessage = string.Format(" ارسال شناسه پرداخت در تراکنش قبض مجاز نیست  {0}", jResult.responseCode);
                            break;

                        case "928":
                            errorMessage = string.Format(" مبلغ مبادله شده نا معتبر می باشد  {0}", jResult.responseCode);
                            break;

                        case "929":
                            errorMessage = string.Format(" شناسه پرداخت ارائه شده با توجه به الگوریتم متناظر نا معتبر می باشد  {0}", jResult.responseCode);
                            break;

                        case "930":
                            errorMessage = string.Format(" کد ملی ارائه شده نا معتبر می باشد  {0}", jResult.responseCode);
                            break;

                        case "96":
                            errorMessage = string.Format(" General failure  {0}", jResult.responseCode);
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error in tejarat payment gateway:{ex.Message}", ex);
        }

        return RedirectToRoute("Checkout/PaymentError", new { orderId = id });
    }

    protected override async Task<VerifyingResult> VerifyPayment(Core.Domain.Payment payment, Order order, TejaratPayResultModel model = null)
    {
        await _logger.InformationAsync("Tejarat Pay Result:" + JsonSerializer.Serialize(model));

        var payModel = new PayResultModel()
        {
            MaskedCardNumber = model.maskedPan,
            TeminalNo = model.acceptorId,
            ReferenceNumber = model.retrievalReferenceNumber,
            TrackId = model.systemTraceAuditNumber,
            ResultCode = model.responseCode,
        };

        if (model.responseCode != "00")
        {
            if (model.responseCode == "17")
            {
                return GenerateFailedResult(ErrorCode.CanceledByUser, payModel);
            }
            else
            {
                return GenerateFailedResult(ErrorCode.GatewayError, payModel);
            }
        }

        if (long.Parse(model.Amount) != Convert.ToInt64(payment.Amount))
        {
            await _logger.ErrorAsync("error in tejarat payment gateway: AmountValueNotEqual");

            return GenerateFailedResult(ErrorCode.AmountValueNotEqual, payModel);

        }

        try
        {
            var requestVerify = new RequestVerify
            {
                Amount = long.Parse(model.Amount),
                responseCode = model.responseCode,
                retrievalReferenceNumber = model.retrievalReferenceNumber,
                systemTraceAuditNumber = model.systemTraceAuditNumber,
                terminalId = model.acceptorId,
                tokenIdentity = model.token
            };

            await _logger.InformationAsync("Tejarat Verify Request:" + JsonSerializer.Serialize(model));

            var result = await _paymentApiCaller.PostAsync<RequestVerify, VerifyResult>(Constant.VerifyUrl, requestVerify);

            await _logger.InformationAsync("Tejarat Verify Response:" + JsonSerializer.Serialize(result));


            if (!result.IsSuccess || result.Data == null)
                return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResult());

            if (result.Data.status)
                GenerateSuccessResult(payModel, result.Data, false);

            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: result.Data);

        }

        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Tejarat verify has error", ex);
            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResult());

        }

    }

    #endregion

    #region Utilities


    private string PreparePOSTFormSaman(string url, NameValueCollection data)
    {
        //Set a name for the form
        string formID = "PostForm";

        //Build the form using the specified data to be posted.
        StringBuilder strForm = new StringBuilder();
        strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\" referrerpolicy=\"origin\">");
        foreach (string key in data)
        {
            strForm.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
        }
        strForm.Append("</form>");
        return strForm.ToString();

    }

    #endregion
}
