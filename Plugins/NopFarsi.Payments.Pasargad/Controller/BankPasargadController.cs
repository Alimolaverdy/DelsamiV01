using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Helpers;
using ZarIran.Plugin.Payment.Core.Controllers;
using ZarIran.Plugin.Payment.Core.Domain;
using ZarIran.Plugin.Payment.Core.Enums;
using ZarIran.Plugin.Payment.Core.Models;
using ZarIran.Plugin.Payment.Core.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Framework.Mvc.Filters;
using NopFarsi.Payments.Pasargad.Models.PasargadPayments;
using NopFarsiIR.Plugin.Payments.BankPasargad.Models;
using NopFarsiIR.Plugin.Payments.BankPasargad.Models.PasargadPayments;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace NopFarsiIR.Plugin.Payments.BankPasargad.Controller;

public partial class BankPasargadController : BaseNopPartsPaymentController<PasargadPayResultModel>
{
    #region Fields

    private const string ConfigureViewName = "~/Plugins/NopFarsi.Payments.Pasargad/Views/Configure.cshtml";
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly ICurrencyService _currencyService;
    private readonly IWebHelper _webHelper;
    private readonly BankPasargadPaymentSettings _bankPasargadPaymentSettings;
    private readonly ICustomerService _customerService;
    private readonly IPaymentApiCaller _paymentApiCaller;

    protected override PaymentGateway paymentGateway => PaymentGateway.Pasargad;

    #endregion

    #region Ctor

    public BankPasargadController(
        ISettingService settingService,
        IPaymentApiCaller paymentApiCaller,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        ICurrencyService currencyService,
        INotificationService notificationService,
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        ILogger logger,
        IWebHelper webHelper,
        BankPasargadPaymentSettings bankPasargadPaymentSettings,
        IPaymentService paymentService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        CoreSettings coreSetttings) : base(paymentService,
                                                             orderService,
                                                             orderProcessingService,
                                                             logger,
                                                             notificationService,
                                                             localizationService,
                                                             eventPublisher,
                                                             coreSetttings)
    {
        _settingService = settingService;
        _paymentApiCaller = paymentApiCaller;
        _storeContext = storeContext;
        _currencyService = currencyService;
        _webHelper = webHelper;
        _bankPasargadPaymentSettings = bankPasargadPaymentSettings;
        _customerService = customerService;
    }

    #endregion

    #region Methods


    public async Task<ActionResult> Pay(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            BankPasargadPaymentSettings bankPasargadPaymentSettings = _settingService.LoadSetting<BankPasargadPaymentSettings>(activeStoreScopeConfiguration);
            var body = new TokenRequest() { username = bankPasargadPaymentSettings.Username, password = bankPasargadPaymentSettings.Password };
            var result = await _paymentApiCaller.PostAsync<TokenRequest, TokenResult>("https://pep.shaparak.ir/dorsa1/token/getToken", body);

            if (!result.IsSuccess || result.Data == null || result.Data.resultCode != 0)
            {
                await _logger.ErrorAsync($"Error in pasargad payment gateway purchase method - error message: {result.ErrorMessage} - status code:{result.StatusCode}");

                return RedirectToRoute("Checkout/PaymentError", new { orderId = id });

            }

            var amount = (order.OrderTotal);
            if (bankPasargadPaymentSettings.IsToman)
                amount = amount * 10;

            var cur = await _currencyService.GetCurrencyByIdAsync(bankPasargadPaymentSettings.CurrencyId);
            amount = _currencyService.ConvertCurrency(amount, cur.Rate);

            var payment = new Payment
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                SourceType = SourceType.Order,
                Gateway = PaymentGateway.Pasargad,
                Amount = Convert.ToInt64(amount),
                CreatedDateTime = DateTime.UtcNow,
                Status = PaymentStatus.Initial,
                TeminalNo = bankPasargadPaymentSettings.TerminalCode,
                PaymentType = PaymentType.IPG
            };

            await _paymentService.InsertAsync(payment);

            var currentStoreUrl = _webHelper.GetStoreLocation();
            if (currentStoreUrl.EndsWith("/"))
                currentStoreUrl = currentStoreUrl.Substring(0, currentStoreUrl.Length - 1);

            var purchaseRequest = new purchaseRequest()
            {
                amount = Convert.ToInt64(amount),
                callbackApi = currentStoreUrl + $"/BankPasargad/VerifyGet?paymentId={payment.Id}",
                //description = "سفارش شماره:" + order.Id,
                invoice = "" + payment.Id,//order.Id,
                invoiceDate = order.CreatedOnUtc.Year + "-" + order.CreatedOnUtc.Month.ToString("00") + "-" + order.CreatedOnUtc.Day.ToString("00"),//.ToString("yyyy-MM-dd"),
                serviceCode = 8,
                terminalNumber = int.Parse(bankPasargadPaymentSettings.TerminalCode),

            };

            if (ValidationHelper.MobilePhoneNumberIsValid(customer.Username))
                purchaseRequest.mobileNumber = customer.Username;

            var purchaseResult = await _paymentApiCaller.PostAsync<purchaseRequest, purchaseResult>("https://pep.shaparak.ir/dorsa1/api/payment/purchase",
                                                                                             purchaseRequest,
                                                                                             result.Data.token);
            if (!purchaseResult.IsSuccess || purchaseResult.Data is null)
            {
                await _logger.ErrorAsync($"Error in pasargad payment gateway purchase method {purchaseResult.ErrorMessage}");

                return RedirectToRoute("Checkout/PaymentError", new { orderId = id });
            }

            payment.ResultCode = purchaseResult.Data.resultCode.ToString();

            if (purchaseResult.Data != null && purchaseResult.Data.resultCode == 0)
            {
                var res = purchaseResult.Data.data;
                order.AuthorizationTransactionId = res.urlId;
                await _orderService.UpdateOrderAsync(order);

                payment.UrlId = res.urlId;
                payment.Status = PaymentStatus.InProgress;

                await _paymentService.UpdateAsync(payment);

                return base.RedirectToAction("RedirectPage", new { url = res.url });
            }


            await _paymentService.UpdateAsync(payment);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error in pasargad payment gateway:{ex.Message}", ex);
        }

        return RedirectToRoute("Checkout/PaymentError", new { orderId = id });

    }

    protected override async Task<VerifyingResult> VerifyPayment(Payment payment, Order order, PasargadPayResultModel model = null)
    {
        try
        {
            var body = new TokenRequest()
            {
                username = _bankPasargadPaymentSettings.Username,
                password = _bankPasargadPaymentSettings.Password
            };

            var tokenResult = await _paymentApiCaller.PostAsync<TokenRequest, TokenResult>("https://pep.shaparak.ir/dorsa1/token/getToken", body);
            await _logger.InformationAsync("Pasargad Token Response:" + JsonSerializer.Serialize(tokenResult));

            var verifyRequest = new VerifyRequest()
            {
                invoice = "" + payment.Id,
                urlId = payment.UrlId
            };

            await _logger.InformationAsync("Pasargad Verify Request:" + JsonSerializer.Serialize(verifyRequest));

            var verifyResult = await _paymentApiCaller.PostAsync<VerifyRequest, VerifyResponse>("https://pep.shaparak.ir/dorsa1/api/payment/verify-payment", verifyRequest, tokenResult.Data.token);

            //if (verifyResult.Data.data.amount != (double)payment.Amount)
            //{
            //    await _logger.ErrorAsync("error in pasargad payment gateway: AmountValueNotEqual");

            //    return GenerateFailedResult(ErrorCode.AmountValueNotEqual, verifyModel: verifyResult.Data);
            //}

            await _logger.InformationAsync("Pasargad Verify Response:" + JsonSerializer.Serialize(verifyResult));

            var payModel = new PayResultModel()
            {
                ResultCode = verifyResult.Data.resultCode.ToString(),
                ReferenceNumber = model.ReferenceNumber,
                TrackId = model.TrackId.ToString(),
                MaskedCardNumber = "",
                TeminalNo = _bankPasargadPaymentSettings.TerminalCode,
            };

            if (!verifyResult.IsSuccess || verifyResult.Data == null)
                return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResponse());


            if (verifyResult.Data.resultCode != 0)
            {
                if (model.Status == "cancel")
                {
                    return GenerateFailedResult(ErrorCode.CanceledByUser, verifyModel: verifyResult.Data);
                }
                else
                {
                    return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: verifyResult.Data);
                }
            }

            if (!_bankPasargadPaymentSettings.ConfirmTransaction)
            {

                var confirmResult = await _paymentApiCaller.PostAsync<VerifyRequest, ConfirmResponse>("https://pep.shaparak.ir/dorsa1/api/payment/confirm-transactions", verifyRequest, tokenResult.Data.token);

                await _logger.InformationAsync("Pasargad Confirm Response:" + JsonSerializer.Serialize(confirmResult));


                if (confirmResult.IsSuccess && confirmResult.Data == null && confirmResult.Data.resultCode == 0)
                {
                    payModel.ResultCode = confirmResult.Data.resultCode.ToString();
                    payModel.MaskedCardNumber = confirmResult.Data.data.maskedCardNumber;
                    payModel.HashedCardNumber = confirmResult.Data.data.hashedCardNumber;

                    return GenerateSuccessResult(payModel, verifyResult.Data, false, confirmResult.Data);
                }


            }

            return GenerateSuccessResult(payModel, verifyResult.Data, false);

        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error in verify from Pasargad payment", ex);
            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new VerifyResponse());
        }
    }








    [AuthorizeAdmin(true)]
    [Area("Admin")]
    public async Task<ActionResult> Configure()
    {
        int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        BankPasargadPaymentSettings bankPasargadPaymentSettings = _settingService.LoadSetting<BankPasargadPaymentSettings>(activeStoreScopeConfiguration);
        ConfigurationModel configurationModel = new ConfigurationModel
        {
            Username = bankPasargadPaymentSettings.Username,
            Password = bankPasargadPaymentSettings.Password,
            TerminalCode = bankPasargadPaymentSettings.TerminalCode,
            IsToman = bankPasargadPaymentSettings.IsToman,
            CurrencyId = bankPasargadPaymentSettings.CurrencyId,
            ActiveStoreScopeConfiguration = activeStoreScopeConfiguration,
            ConfirmTransaction = bankPasargadPaymentSettings.ConfirmTransaction,
        };

        if (activeStoreScopeConfiguration > 0)
        {
            configurationModel.PasswordOverrideForStore = _settingService.SettingExists<BankPasargadPaymentSettings, string>(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.Password, activeStoreScopeConfiguration);
            configurationModel.UsernameOverrideForStore = _settingService.SettingExists<BankPasargadPaymentSettings, string>(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.Username, activeStoreScopeConfiguration);
            configurationModel.UserPasswordOverrideForStore = _settingService.SettingExists<BankPasargadPaymentSettings, string>(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.TerminalCode, activeStoreScopeConfiguration);
            configurationModel.IsTomanOverrideForStore = _settingService.SettingExists<BankPasargadPaymentSettings, bool>(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.IsToman, activeStoreScopeConfiguration);
            configurationModel.IsConfirmTransactionOverrideForStore = _settingService.SettingExists<BankPasargadPaymentSettings, bool>(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.ConfirmTransaction, activeStoreScopeConfiguration);
        }
        var cur = await _currencyService.GetAllCurrenciesAsync();
        IList<SelectListItem> selectListItems = new List<SelectListItem>();
        foreach (var item in cur)
        {

            selectListItems.Add(new SelectListItem(item.Name, item.Id.ToString(), item.Id == bankPasargadPaymentSettings.CurrencyId));
        }
        ViewBag.CurrencyItems = selectListItems;
        return View("~/Plugins/NopFarsi.Payments.Pasargad/Views/Configure.cshtml", configurationModel);
    }

    [HttpPost]
    [AuthorizeAdmin(true)]
    [Area("Admin")]
    public async Task<ActionResult> Configure(ConfigurationModel model, bool IsToman)
    {
        bool flag = !base.ModelState.IsValid;
        ActionResult result;
        if (flag)
        {
            result = await Configure();
        }
        else
        {
            int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            BankPasargadPaymentSettings bankPasargadPaymentSettings = _settingService.LoadSetting<BankPasargadPaymentSettings>(activeStoreScopeConfiguration);
            bankPasargadPaymentSettings.Password = model.Password;
            bankPasargadPaymentSettings.Username = model.Username;
            bankPasargadPaymentSettings.TerminalCode = model.TerminalCode;
            bankPasargadPaymentSettings.IsToman = IsToman;
            bankPasargadPaymentSettings.CurrencyId = model.CurrencyId;
            bankPasargadPaymentSettings.ConfirmTransaction = model.ConfirmTransaction;
            await _settingService.SaveSettingOverridablePerStoreAsync(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.Password, model.PasswordOverrideForStore, activeStoreScopeConfiguration, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.Username, model.UsernameOverrideForStore, activeStoreScopeConfiguration, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.TerminalCode, model.UserPasswordOverrideForStore, activeStoreScopeConfiguration, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.IsToman, IsToman, activeStoreScopeConfiguration, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.CurrencyId, true, activeStoreScopeConfiguration, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankPasargadPaymentSettings, (BankPasargadPaymentSettings x) => x.ConfirmTransaction, true, activeStoreScopeConfiguration, false);
            _settingService.ClearCache();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"), true);
            result = await Configure();
        }
        return result;
    }

    public IActionResult RedirectPage(string url)
    {
        ViewData["Url"] = url;

        return View("~/Plugins/NopFarsi.Payments.Pasargad/Views/RedirectPage.cshtml");
    }


    #endregion
}
