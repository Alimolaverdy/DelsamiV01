using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Delsami.Plugin.Payment.Core.Domain;
using Delsami.Plugin.Payment.Core.Enums;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using NopFarsiIR.Payments.Mellat.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Customers;
using Delsami.Plugin.Misc.Core.Helpers;

namespace NopFarsiIR.Payments.Mellat
{
    public class MellatPaymentProcessor : IPaymentMethod, IPlugin
    {
        private readonly ICurrencyService _currencySettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly MellatPaymentSettings _MellatPaymentSettings;
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;
        private readonly Delsami.Plugin.Payment.Core.Services.IPaymentService _paymentService;

        public MellatPaymentProcessor(ICurrencyService currencySettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IHttpContextAccessor httpContextAccessor,
            IWebHelper webHelper, ILogger logger,
            MellatPaymentSettings mellatPaymentSettings,
            ICustomerService customerService,
            Delsami.Plugin.Payment.Core.Services.IPaymentService paymentService)
        {
            _currencySettings = currencySettings;
            _httpContextAccessor = httpContextAccessor;
            _checkoutAttributeParser = checkoutAttributeParser;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _taxService = taxService;
            _webHelper = webHelper;
            _MellatPaymentSettings = mellatPaymentSettings;
            _logger = logger;
            _customerService = customerService;
            _paymentService = paymentService;
        }


        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/NopFarsiPaymentMellat/Configure";
        }


        public async Task InstallAsync()
        {

            MellatPaymentSettings mellatPaymentSettings = new MellatPaymentSettings
            {
                TerminalId = "1",
                UserName = "1",
                UserPassword = "1",
                OrderId = 1,
                Toman = true
            };
            await _settingService.SaveSettingAsync<MellatPaymentSettings>(mellatPaymentSettings, 0);

            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.OrderId", "شماره درخواست بانک ملت", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("plugins.payments.mellat.paymentmethoddescription", "پرداخت توسط کلیه بانک های عضو شتاب", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.OrderId.Hint", "شماره ای غیر تکراری که در هر بار قبل از رفتن به درگاه بانک ارسال می شود", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.TerminalId", "شماره ترمینال", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.TerminalId.Hint", "شماره ترمینال بانک ملت", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.UserName", "نام کاربری", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.UserName.Hint", "نام کاربری", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.UserPassword", "رمز عبور", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.UserPassword.Hint", "رمز عبور", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ActivationCode", "کد فعالسازی", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ActivationCode.Hint", "کد فعالسازی", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.SystemCode", "کد سیستم", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.SystemCode.Hint", "کد سیستم", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.Toman", "استفاده از تومان", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.Toman.Hint", "در صورتی که قیمت کالاها بر حسب تومان می باشد ، این گزینه را تیک بزنید", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.RedirectionTip", "برای تکمیل سفارش به درگاه پرداخت هدایت خواهید شد", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.BusinessEmail", "Business Email", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.BusinessEmail.Hint", "Specify your Mellat business email.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFee", "هزینه های اضافی", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFee.Hint", "هزینه های اضافی برای مطالبه از مشتریان خود وارد نمایید", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFeePercentage", "هزینه اضافی. از درصد استفاده نمایید", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFeePercentage.Hint", "تعیین اینکه آیا  درصد هزینه های اضافی به کل سفارش اعمال شود. اگر فعال نشود ، یک مقدار ثابت استفاده می شود", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage", "بازگشت به صفحه جزئیات سفارش", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"return to store\" link on Mellat site WITHOUT completing a payment", null);
            await Task.CompletedTask;
        }

        public async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<MellatPaymentSettings>();

            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.OrderId");
            await _localizationService.DeleteLocaleResourceAsync("plugins.payments.mellat.paymentmethoddescription");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.TerminalId");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.UserName");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.UserPassword");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ActivationCode");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.SystemCode");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.Toman");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.RedirectionTip");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.BusinessEmail");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.BusinessEmail.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFee");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFee.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFeePercentage");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.AdditionalFeePercentage.Hint");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage");
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Payments.Mellat.Fields.ReturnFromMellatWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            await Task.CompletedTask;
        }

        public IList<string> ValidatePaymentForm(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            if (form == null)
                throw new ArgumentException("");

            //try to get errors
            if (form.TryGetValue("Errors", out StringValues errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                return (new[] { errorsString.ToString() });

            return new List<string>();
        }



        public string GetPublicViewComponentName()
        {
            return "PaymentInfo";
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var storeUrl = _webHelper.GetStoreLocation();
            if (!storeUrl.EndsWith("/"))
                storeUrl += "/";

            var errorPageUrl = $"{storeUrl}Checkout/PaymentError?orderId={postProcessPaymentRequest.Order.Id}";
            try
            {
                decimal value = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
                long num = (long)postProcessPaymentRequest.Order.Id;
                string terminalId = _MellatPaymentSettings.TerminalId;
                string userName = _MellatPaymentSettings.UserName;
                string userPassword = _MellatPaymentSettings.UserPassword;
                int num2 = 1;
                if (_MellatPaymentSettings.OrderId.ToString() != "")
                {
                    num2 = _MellatPaymentSettings.OrderId;
                }
                long orderId = Convert.ToInt64(_MellatPaymentSettings.OrderId);
                _MellatPaymentSettings.OrderId = num2 + 1;
                await _settingService.SaveSettingAsync<MellatPaymentSettings>(_MellatPaymentSettings, 0);
                string prefixChar = _webHelper.GetStoreLocation().EndsWith('/') ? "" : "/";
                if (terminalId == "" || userName == "" || userPassword == "")
                {
                    _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
                }
                else
                {
                    long num3 = Convert.ToInt64(value);
                    if (_MellatPaymentSettings.Toman)
                    {
                        num3 *= 10L;
                    }

                    var payment = new Payment
                    {
                        OrderId = postProcessPaymentRequest.Order.Id,
                        CustomerId = postProcessPaymentRequest.Order.CustomerId,
                        SourceType = SourceType.Order,
                        Gateway = PaymentGateway.Mellat,
                        Amount = num3,
                        CreatedDateTime = DateTime.UtcNow,
                        Status = PaymentStatus.Initial,
                        TeminalNo = _MellatPaymentSettings.TerminalId,
                        PaymentType = PaymentType.IPG
                    };
                    await _paymentService.InsertAsync(payment);
                    string additionalData = "خرید از فروشگاه اینترنتی " + num;
                    string callBackUrl = _webHelper.GetStoreLocation() + prefixChar + $"NopFarsiPaymentMellat/Verify?paymentId={payment.Id}";
                    string localDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                    string localTime = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                    var mobileNo = (await _customerService.GetCustomerByIdAsync(payment.CustomerId)).Username;
                    if (ValidationHelper.MobilePhoneNumberIsValid(mobileNo))
                        mobileNo = "98" + mobileNo.Substring(1);

                    var paymentGatewayImplService = new bpm.shaparak.PaymentGatewayClient();

                    paymentGatewayImplService.Endpoint.Binding.SendTimeout = TimeSpan.FromSeconds(30);
                    paymentGatewayImplService.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromSeconds(30);
                    paymentGatewayImplService.Endpoint.Binding.OpenTimeout = TimeSpan.FromSeconds(15);
                    paymentGatewayImplService.Endpoint.Binding.CloseTimeout = TimeSpan.FromSeconds(15);

                    var request = new
                    {
                        terminalId,
                        userName,
                        userPassword,
                        payment.Id,
                        num3,
                        localDate,
                        localTime,
                        additionalData,
                        callBackUrl,
                        payment.CustomerId
                    };

                    var requestStr = Newtonsoft.Json.JsonConvert.SerializeObject(request);

                    await _logger.InformationAsync("1-mellat request object:" + requestStr);

                    string text = paymentGatewayImplService.bpPayRequestAsync(long.Parse(terminalId), userName, userPassword, payment.Id, num3, localDate, localTime, additionalData, callBackUrl, payment.CustomerId /*long.Parse("0")*/).Result.Body.@return;

                    await _logger.InformationAsync("2-mellat response:" + text);

                    string[] array = text.Split(new char[]
                    {
                        ','
                    });
                    if (array[0] == "0")
                    {
                        postProcessPaymentRequest.Order.AuthorizationTransactionId = array[1];
                        payment.UrlId = array[1];
                        payment.Status = PaymentStatus.InProgress;
                        await _paymentService.UpdateAsync(payment);
                        _httpContextAccessor.HttpContext.Response.Redirect(
                     $"{_webHelper.GetStoreLocation()}NopFarsiPaymentMellat/Pay?result={array[1]}&MobileNo={mobileNo}");
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error in mellat payment gateway:{ex.Message}", ex);

                _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
            }
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(0m);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult capture = new CapturePaymentResult();
            capture.AddError("Capture method not supported.");
            return Task.FromResult(capture);
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult refund = new RefundPaymentResult();
            refund.AddError("Refund method not supported.");
            return Task.FromResult(refund);
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult voidPay = new VoidPaymentResult();
            voidPay.AddError("Void method not supported.");
            return Task.FromResult(voidPay);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult p = new ProcessPaymentResult();
            p.AddError("Recurring payment not supported.");
            return Task.FromResult(p);
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult c = new CancelRecurringPaymentResult();
            c.AddError("Recurring payment not supported.");
            return Task.FromResult(c);
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException("order");
            }
            return Task.FromResult((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds >= 5.0);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            if (form == null)
            {
                throw new ArgumentException(nameof(form));
            }

            //try to get errors
            if (form.TryGetValue("Errors", out StringValues errorsString) && !StringValues.IsNullOrEmpty(errorsString))
            {
                var errorList = new List<string> { errorsString.ToString() };
                return Task.FromResult<IList<string>>(errorList);
            }

            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.Mellat.PaymentMethodDescription");
        }

        public async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await Task.CompletedTask;
        }

        public async Task PreparePluginToUninstallAsync()
        {
            await Task.CompletedTask;
        }

        public Type GetPublicViewComponent()
        {
            return typeof(PaymentInfoViewComponent);
        }

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return 0;
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return true;
            }
        }

        public PluginDescriptor PluginDescriptor { get; set; }
    }
}
