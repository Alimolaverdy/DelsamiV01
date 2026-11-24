using bpm.shaparak;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Events;
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
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using NopFarsiIR.Payments.Mellat.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace NopFarsiIR.Payments.Mellat.Controllers;


public class NopFarsiPaymentMellatController : BaseNopPartsPaymentController<MellatPayResultModel>
{

    #region Fields

    private readonly ISettingService _settingService;
    private readonly MellatPaymentSettings _MellatPaymentSettings;
    private readonly IPermissionService _permissionService;
    private readonly IStoreContext _storeContext;
    private readonly IPaymentResilienceService _paymentResilienceService;

    protected override PaymentGateway paymentGateway => PaymentGateway.Tejarat;

    #endregion

    #region Ctor

    public NopFarsiPaymentMellatController(
                                           ISettingService settingService,
                                           IOrderService orderService,
                                           IOrderProcessingService orderProcessingService,
                                           ILogger logger,
                                           ILocalizationService localizationService,
                                           IPaymentResilienceService paymentResilienceService,
                                           MellatPaymentSettings MellatPaymentSettings,
                                           IPermissionService permissionService,
                                           IStoreContext storeContext,
                                           INotificationService notificationService,
                                           IPaymentService paymentService,
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
        _settingService = settingService;
        _MellatPaymentSettings = MellatPaymentSettings;
        _permissionService = permissionService;
        _storeContext = storeContext;
        _paymentResilienceService = paymentResilienceService;
    }
    #endregion

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure()
    {
        //whether user has the authority
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();


        int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        MellatPaymentSettings mellatPaymentSettings = await this._settingService.LoadSettingAsync<MellatPaymentSettings>(activeStoreScopeConfiguration);
        ConfigurationModel configurationModel = new ConfigurationModel
        {
            TerminalId = mellatPaymentSettings.TerminalId,
            UserName = mellatPaymentSettings.UserName,
            UserPassword = mellatPaymentSettings.UserPassword,
            OrderId = mellatPaymentSettings.OrderId,
            Toman = mellatPaymentSettings.Toman,
            AdditionalFee = mellatPaymentSettings.AdditionalFee,
            AdditionalFeePercentage = mellatPaymentSettings.AdditionalFeePercentage,
            ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = mellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage,
            ActiveStoreScopeConfiguration = activeStoreScopeConfiguration
        };
        if (activeStoreScopeConfiguration > 0)
        {
            configurationModel.TerminalId_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.TerminalId, activeStoreScopeConfiguration);
            configurationModel.UserName_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserName, activeStoreScopeConfiguration);
            configurationModel.UserPassword_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserPassword, activeStoreScopeConfiguration);
            configurationModel.OrderId_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, int>(mellatPaymentSettings, (MellatPaymentSettings x) => x.OrderId, activeStoreScopeConfiguration);
            configurationModel.Toman_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.Toman, activeStoreScopeConfiguration);
            configurationModel.AdditionalFee_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, decimal>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFee, activeStoreScopeConfiguration);
            configurationModel.AdditionalFeePercentage_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfiguration);
            configurationModel.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore = await this._settingService.SettingExistsAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage, activeStoreScopeConfiguration);
        }

        return View("~/Plugins/NopFarsiIR.Payments.Mellat/Views/PaymentMellat/Configure.cshtml", configurationModel);
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        //whether user has the authority
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();


        if (!base.ModelState.IsValid)
        {
            return await Configure();
        }
        else
        {
            int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            MellatPaymentSettings mellatPaymentSettings = await this._settingService.LoadSettingAsync<MellatPaymentSettings>(activeStoreScopeConfiguration);
            mellatPaymentSettings.TerminalId = model.TerminalId;
            mellatPaymentSettings.UserName = model.UserName;
            mellatPaymentSettings.UserPassword = model.UserPassword;
            mellatPaymentSettings.OrderId = model.OrderId;
            mellatPaymentSettings.Toman = model.Toman;
            mellatPaymentSettings.AdditionalFee = model.AdditionalFee;
            mellatPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            mellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = model.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage;
            if (model.TerminalId_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.TerminalId, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.TerminalId, activeStoreScopeConfiguration);
            }
            if (model.UserName_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserName, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserName, activeStoreScopeConfiguration);
            }
            if (model.UserPassword_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserPassword, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, string>(mellatPaymentSettings, (MellatPaymentSettings x) => x.UserPassword, activeStoreScopeConfiguration);
            }
            if (model.OrderId_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, int>(mellatPaymentSettings, (MellatPaymentSettings x) => x.OrderId, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, int>(mellatPaymentSettings, (MellatPaymentSettings x) => x.OrderId, activeStoreScopeConfiguration);
            }

            if (model.Toman_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.Toman, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.Toman, activeStoreScopeConfiguration);
            }

            if (model.AdditionalFee_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, decimal>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFee, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, decimal>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFee, activeStoreScopeConfiguration);
            }
            if (model.AdditionalFeePercentage_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.AdditionalFeePercentage, activeStoreScopeConfiguration);
            }
            if (model.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore || activeStoreScopeConfiguration == 0)
            {
                await this._settingService.SaveSettingAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage, activeStoreScopeConfiguration, false);
            }
            else if (activeStoreScopeConfiguration > 0)
            {
                await this._settingService.DeleteSettingAsync<MellatPaymentSettings, bool>(mellatPaymentSettings, (MellatPaymentSettings x) => x.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage, activeStoreScopeConfiguration);
            }
            await this._settingService.ClearCacheAsync();

        }
        this._notificationService.SuccessNotification("با موفقیت ذخیره شد.");
        return View("~/Plugins/NopFarsiIR.Payments.Mellat/Views/PaymentMellat/Configure.cshtml", model);
    }





    public async void UpdateOrderId(int? orderId, int? installOrderId)
    {
        MellatPaymentSettings mellatPaymentSettings = new MellatPaymentSettings
        {
            OrderId = (orderId ?? this._MellatPaymentSettings.OrderId),
            TerminalId = this._MellatPaymentSettings.TerminalId,
            UserName = this._MellatPaymentSettings.UserName,
            UserPassword = this._MellatPaymentSettings.UserPassword,
            Toman = this._MellatPaymentSettings.Toman,
            AdditionalFee = this._MellatPaymentSettings.AdditionalFee,
            AdditionalFeePercentage = this._MellatPaymentSettings.AdditionalFeePercentage,
            ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage = this._MellatPaymentSettings.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage
        };
        await this._settingService.SaveSettingAsync<MellatPaymentSettings>(mellatPaymentSettings, 0);
    }

    public IActionResult Pay(string result, string MobileNo)
    {
        ViewBag.result = result;
        ViewBag.MobileNo = MobileNo;
        return View("~/Plugins/NopFarsiIR.Payments.Mellat/Views/PaymentMellat/Pay.cshtml");
    }

    internal static bool RoutePersianCalneder(HttpRequest param)
    {
        return true;
    }

    protected override async Task<VerifyingResult> VerifyPayment(Payment payment, Order order, MellatPayResultModel model = null)
    {

        await _logger.InformationAsync("3-mellat redirect object:" + JsonSerializer.Serialize(model));

        var payModel = new PayResultModel()
        {
            ResultCode = model.resCode,
            ReferenceNumber = model.refId,
            MaskedCardNumber = model.cardHolderPan,
            Amount = model.finalAmount,
            TrackId = model.saleReferenceId,
            TeminalNo = "",
            AdditionalData = model.creditCardSaleResponseDetail,
        };

        if (string.IsNullOrEmpty(model.resCode)
            || string.IsNullOrEmpty(model.refId)
            || string.IsNullOrEmpty(model.saleOrderId)
            || string.IsNullOrEmpty(model.saleReferenceId)
            || model.resCode != "0")
        {
            if (model.resCode == "17")
            {
                return GenerateFailedResult(ErrorCode.CanceledByUser, payModel);
            }
            else
            {
                return GenerateFailedResult(ErrorCode.GatewayError, payModel);
            }
        }

        //if (decimal.Parse(model.finalAmount) != payment.Amount)
        //{
        //    await _logger.ErrorAsync("error in tejarat payment gateway: AmountValueNotEqual");

        //    return GenerateFailedResult(ErrorCode.AmountValueNotEqual, payModel);
        //}

        try
        {
            string terminalId = _MellatPaymentSettings.TerminalId;
            string userName = _MellatPaymentSettings.UserName;
            string userPassword = _MellatPaymentSettings.UserPassword;

            var paymentGatewayClient = new PaymentGatewayClient();

            paymentGatewayClient.Endpoint.Binding.SendTimeout = TimeSpan.FromSeconds(300);
            paymentGatewayClient.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromSeconds(300);
            paymentGatewayClient.Endpoint.Binding.OpenTimeout = TimeSpan.FromSeconds(150);
            paymentGatewayClient.Endpoint.Binding.CloseTimeout = TimeSpan.FromSeconds(150);

            await _logger.InformationAsync("4-mellat verify request:" + JsonSerializer.Serialize(new
            {
                terminalId,
                userName,
                userPassword,
                payment.Id,
                model.saleOrderId,
                model.saleReferenceId,
            }));

            var verifyResponse = await _paymentResilienceService.ExecuteAsync("mellat verify request", async ct =>
            {
                return await paymentGatewayClient.bpVerifyRequestAsync(long.Parse(terminalId),
                                                                       userName,
                                                                       userPassword,
                                                                       payment.Id,
                                                                       long.Parse(model.saleOrderId),
                                                                       long.Parse(model.saleReferenceId));
            });


            await _logger.InformationAsync("4-mellat verify response:" + verifyResponse.Body.@return);

            if (verifyResponse.Body.@return != "0")
                return GenerateFailedResult(ErrorCode.GatewayError);

            await _logger.InformationAsync("5-mellat settle request:" + JsonSerializer.Serialize(new
            {
                terminalId,
                userName,
                userPassword,
                payment.Id,
                model.saleOrderId,
                model.saleReferenceId
            }));

            var settleResponse = await _paymentResilienceService.ExecuteAsync("mellat settle request", async ct =>
            {
                return await paymentGatewayClient.bpSettleRequestAsync(long.Parse(terminalId),
                                                                       userName,
                                                                       userPassword,
                                                                       payment.Id,
                                                                       long.Parse(model.saleOrderId),
                                                                       long.Parse(model.saleReferenceId));
            });

            await _logger.InformationAsync("6-mellat settle response:" + settleResponse.Body.@return);

            if (settleResponse.Body.@return != "0")
            {
                return GenerateFailedResult(ErrorCode.GatewayError);
            }

            return GenerateSuccessResult(payModel,
                                         new MellatVerifyResponseModel { ResultCode = verifyResponse.Body.@return },
                                         false,
                                         new MellatVerifyResponseModel { ResultCode = settleResponse.Body.@return });

        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error in Mellat Pay Verify or Settle. PaymentId: {payment.Id}, Error: {ex.Message}", ex);
            return GenerateFailedResult(ErrorCode.GatewayError, verifyModel: new MellatVerifyResponseModel());

        }

    }


    public ActionResult PaymentResult(string result)
    {
        base.ViewBag.Message = result;
        return base.View("~/Plugins/NopFarsiIR.Plugin.Payments.Mellat/Views/PaymentMellat/PaymentResult.cshtml");
    }

    public ActionResult Error(string ErrorCode, string resCode, int? id)
    {
        if (string.IsNullOrEmpty(ErrorCode))
            ErrorCode = resCode;
        ViewBag.orderId = id;
        ActionResult result;
        if (ErrorCode == "17")
        {
            result = this.Redirect("/orderdetails/" + id.ToString());
        }
        else
        {
            base.ViewBag.ErrorCode = ErrorCode;
            if (ErrorCode == "-2")
            {
                base.ViewBag.result = "شما مجوز استفاده از ماژول درگاه را ندارید";
            }
            else if (ErrorCode == "-1")
            {
                base.ViewBag.result = "درگاه پیکربندی نشده است. مقادیر شماره ترمینال ، نام کاربری و رمز عبور را وارد نمایید";

            }
            else if (ErrorCode == "11")
            {
                base.ViewBag.result = "شماره کارت نا معتبر است";
            }
            else if (ErrorCode == "12")
            {
                base.ViewBag.result = "موجودی کافی نیست";
            }
            else if (ErrorCode == "13")
            {
                base.ViewBag.result = "رمز نادرست است";
            }
            else if (ErrorCode == "15")
            {
                base.ViewBag.result = "کارت نا معتبر است";
            }
            else if (ErrorCode == "18")
            {
                base.ViewBag.result = "تاریخ انقضای کارت گذشته است";
            }
            else if (ErrorCode == "21")
            {
                base.ViewBag.result = "پذیرنده نا معتبر است";
            }
            else if (ErrorCode == "41")
            {
                base.ViewBag.result = "شماره درخواست تکرای است";
            }
            else
            {
                base.ViewBag.result = Utility.ErrorCode("bpPayRequest", ErrorCode);
            }
            result = base.View("~/Plugins/NopFarsiIR.Payments.Mellat/Views/PaymentMellat/Error.cshtml");
        }
        return result;
    }


}
