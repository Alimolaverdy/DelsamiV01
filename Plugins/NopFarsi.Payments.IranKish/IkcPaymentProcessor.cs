using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using NopFarsi.Payments.IranKish.Components;
using NopFarsi.Payments.IranKish.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.IranKish
{
    public class IkcPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region variable & prop

        private ILocalizationService _localizationService;
        private readonly IPaymentService paymentService;

        private readonly IkcSettings _IkcSettingsSettings;
        private readonly IWebHelper webHelper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISettingService settingService;

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => true;

        #endregion

        public IkcPaymentProcessor(
            ILocalizationService localizationService,
            IPaymentService paymentService,
            IkcSettings IkcSettingsSettings,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            ISettingService settingService
            )
        {
            this._localizationService = localizationService;
            this.paymentService = paymentService;
            this._IkcSettingsSettings = IkcSettingsSettings;
            this.webHelper = webHelper;
            this.httpContextAccessor = httpContextAccessor;
            this.settingService = settingService;
        }


        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            decimal value = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            long num = (long)postProcessPaymentRequest.Order.Id;

            long num3 = Convert.ToInt64(value);
            if (this._IkcSettingsSettings.IsToman)
            {
                num3 *= 10L;
            }

            string callBackUrl = this.webHelper.GetStoreLocation(false) + "IkcNopFarsi/Verify?OrderId=" + num;

            NameValueCollection datacollection = new NameValueCollection();

            datacollection.Add("ResNum", postProcessPaymentRequest.Order.Id.ToString());

            datacollection.Add("MID", this._IkcSettingsSettings.MerchantId.ToString());


            datacollection.Add("RedirectURL", callBackUrl);

            datacollection.Add("Amount", num3.ToString());

            this.httpContextAccessor.HttpContext.Response.Redirect(this.webHelper.GetStoreLocation() + "IkcNopFarsi/Pay?ResNum=" + postProcessPaymentRequest.Order.Id.ToString()
                + "&MID=" + this._IkcSettingsSettings.MerchantId.ToString() +
                "&Amount=" + num3.ToString() +
                "&RedirectURL=" + callBackUrl);
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult c = new CancelRecurringPaymentResult();
            c.AddError("Recurring payment not supported.");
            return c;
        }




        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }


        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult p = new ProcessPaymentResult();
            p.AddError("Recurring payment not supported.");
            return p;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult refund = new RefundPaymentResult();
            refund.AddError("Refund method not supported.");
            return refund;
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentException(nameof(form));

            StringValues errorsString;
            //try to get errors
            if (form.TryGetValue("Errors", out errorsString) && !StringValues.IsNullOrEmpty(errorsString))
                return (new[] { errorsString.ToString() });

            return new List<string>();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult voidPay = new VoidPaymentResult();
            voidPay.AddError("Void method not supported.");
            return voidPay;
        }

        public override string GetConfigurationPageUrl()
        {
            if (this.PluginDescriptor.Author != "NopFarsi.ir  (MJ Vakili)")
                throw new NotImplementedException();
            return webHelper.GetStoreLocation() + "Admin/IkcNopFarsi/Configure";
        }

        public override async Task InstallAsync()
        {
            IkcSettings SepPaymentSettings = new IkcSettings
            {
                MerchantId = "",

                IsToman = false
            };
            this.settingService.SaveSetting<IkcSettings>(SepPaymentSettings);

            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Sep.Fields.RedirectionTip", " به درگاه ایران کیش متصل خواهید شد.", null);

            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.RedirectionTip", " به درگاه ایران کیش متصل خواهید شد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.MerchantId", "کد درگاه پرداخت", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.MerchantId.Hint", "كدي يكتا و 36 كاراكتري است كه ایران کیش به ازاي هر درخواست درگاه پرداخت به پذيرنده اختصاص می‌دهد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.PayementUrl", "آدرس درگاه پرداخت", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.PayementUrl.Hint", "آدرس اینترنتی‌ای که مشتری برای پرداخت مبلغ فاکتور به آن هدایت می‌شود.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.WebServiceUrl", "آدرس وب سرویس", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.WebServiceUrl.Hint", "آدرس وب سرویس ایران کیش برای ازتباط با درگاه پرداخت.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.IsToman", "محاسبه بر اساس تومان", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.DisablePaymentInfo", "صفحه پرداخت غیر فعال", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.DisablePaymentInfo.Hint", "با این عمل صفحه توضیحات پرداخت غیر فعال می شود. توضیحات پرداخت صفحه ای است که بعد از انتخاب درگاه پرداخت در ناپ نمایش داده می شود.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.Apportionment", "پارامتر تسهیم", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Fields.Apportionment.Hint", "با استفاده از این پارامتر می توانید پرداخت را بین افراد زیر نفع تقسیم کنید. اعداد با ],[ جدا شود و مجموع آن باید صد باشد. در صورتی که از تسهیم استفاده نمی کنید مقداری وارد نکنید.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.PaymentMethodDescription", "پرداخت توسط درگاه ایران کیش", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Verify.SuccessMessage", "صورتحساب با موفقیت پرداخت گردید.\r\nکد پیگیری : {0}", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Verify.FailureMessage", "پردخت  ناموفق بود.\r\nشرح خطا : {0}", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Payment.Description", "بابت خرید فاکتور شماه {0}", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.Payment.FailureMessage", "برقراری ارتباط با سرور ایران کیش امکان پذیر نمی‌باشد.\r\nشرح خطا : {0}", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-1", "اطلاعات ارسالی ناقص است.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-2", "آی‌پی و یا مرچنت کد،  صحیح نیست.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-3", "با توجه به محدودیت‌های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی‌باشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-4", "سطح تایید پذیرنده پایین‌تر از سطح نقره‌ای است.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-11", "درخواست مورد نظر یافت نشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-12", "امکان ویرایش درخواست میسر نمی‌باشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-21", "هیچ نوع عملیات مالی برای این تراکنش یافت نشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-22", "تراکنش ناموفق می‌باشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-33", "رقم تراکنش با رقم پرداخت شده مطابقت ندارد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-34", "سقف تقسیم تراکنش از لحاظ تعداد یا رقم، عبور نموده است.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-40", "اجازه دسترسی به متد مربوطه وجود ندارد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-41", "اطلاعات ارسال شده مربوط به AdditionalData غیرمعتبر می‌باشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-42", "مدت زمان معتبر طول عمر شناسه پرداخت باید بین  30 دقیه تا  45 روز می‌باشد.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.-54", "درخواست مورد نظر آرشیو شده است.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.100", "عملیات با موفقیت انجام گردیده است.", null);
            await _localizationService.AddOrUpdateLocaleResourceAsync("NopFarsi.Ikc.StatusCode.101", "عملیات پرداخت موفق بوده و قبلاً PaymentVerification تراکنش انجام شده است.", null);
            await base.InstallAsync();
            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            await this.settingService.DeleteSettingAsync<IkcSettings>();

            await base.UninstallAsync();
            await Task.CompletedTask;
        }


        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return 0;
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
            return await this._localizationService.GetResourceAsync("NopFarsi.Ikc.PaymentMethodDescription");
        }

        public Type GetPublicViewComponent()
        {
            return typeof(IkcPaymentInfoViewComponent);
        }
    }
}
