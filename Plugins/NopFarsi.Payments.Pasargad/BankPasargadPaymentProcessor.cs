using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NopFarsiIR.Plugin.Payments.BankPasargad
{
    public class BankPasargadPaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        public BankPasargadPaymentProcessor(
    IHttpContextAccessor httpContextAccessor,
    BankPasargadPaymentSettings bankPasargadPaymentSettings,
    ILocalizationService localizationService,
    ISettingService settingService,
    IWebHelper webHelper,
    IGenericAttributeService genericAttributeService,
    IPaymentService paymentService,
    IOrderService orderService,
    IOrderProcessingService orderProcessingService,
    IShoppingCartService shoppingCartService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._bankPasargadPaymentSettings = bankPasargadPaymentSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._genericAttributeService = genericAttributeService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._shoppingCartService = shoppingCartService;
        }



        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            this._httpContextAccessor.HttpContext.Response.Redirect(this._webHelper.GetStoreLocation() + "BankPasargad/Pay/" + postProcessPaymentRequest.Order.Id);
            return Task.CompletedTask;
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult capturePaymentResult = new CapturePaymentResult();
            capturePaymentResult.AddError("Capture method not supported.");
            return Task.FromResult(capturePaymentResult);
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult refundPaymentResult = new RefundPaymentResult();
            refundPaymentResult.AddError("Refund method not supported.");
            return refundPaymentResult;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult voidPaymentResult = new VoidPaymentResult();
            voidPaymentResult.AddError("Void method not supported.");
            return voidPaymentResult;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.AddError("Recurring payment not supported.");
            return processPaymentResult;
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult cancelRecurringPaymentResult = new CancelRecurringPaymentResult();
            cancelRecurringPaymentResult.AddError("Recurring payment not supported.");
            return cancelRecurringPaymentResult;
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public string GetPublicViewComponentName()
        {
            return "BankPasargadPaymentInfo";
        }

        public bool CanRePostProcessPayment(Order order)
        {
            bool flag = order == null;
            if (flag)
            {
                throw new ArgumentNullException("order");
            }
            return (DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds >= 5.0;
        }

        public override async Task InstallAsync()
        {
            BankPasargadPaymentSettings settings = new BankPasargadPaymentSettings
            {
                TerminalCode = "",
            };
            this._settingService.SaveSetting<BankPasargadPaymentSettings>(settings, 0);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.IsToman", "واحد پول شما تومان است؟", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFeePercentage", "محاسبه اضافه قیمت برحسب درصد", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFeePercentage.Hint", "اضافه قیمت هم می‌تواند درصدی از مبلغ فاکتور باشد و هم یک مقدار ثابت.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFee", "اضافه قیمت", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFee.Hint", "اضافه قیمت به مبلغ نهایی فاکتور اضافه می‌گردد و توسط مشتری می‌بایست پرداخت گردد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.ShippableProductRequired", "فعال فقط برای محصولات قابل ارسال", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.ShippableProductRequired.Hint", "درگاه برای محصولات غیرقابل ارسال نیز می‌تواند فعال باشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.Username", "نام كاربري", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PrivateKeyText.Hint", "کلیدي است که فروشنده براي احراز هویت از آن استفاده میکند و تمامی دادههاي ارسالی خود به بانک را با آن کلید، امضاي دیجیتال میکند.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.Password", "رمز عبور", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MerchantCode.Hint", "کدي است که توسط بانک به فروشنده اختصاص مییابد و در حین انجام تراکنش براي شناسایی فروشنده از آن استفاده میگردد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.TerminalCode", "شماره شناسائی ترمینال", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.TerminalCode.Hint", "کدي است که توسط بانک به فروشنده اختصاص مییابد و در حین انجام تراکنش از آن استفاده میگردد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PayerId", "شناسه پرداخت کننده", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PayerId.Hint", "پذیرندگانی که در حساب خود نزد بانک پاسارگاد، درخواست کد شناسه پرداخت داشته‌اند، درصـورت ارسال کد شناسه پرداخت کننده، این فیلد به دارنده کـارت در صـفحه پرداخـت نمایش داده نخواهد شد، در غیر این صورت دارنده کارت باید کد شناسه پرداخت کننده معتبر وارد نماید.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PaymentUrl", "آدرس درگاه پرداخت", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PaymentUrl.Hint", "آدرس اینترنتی‌ای که مشتری برای پرداخت مبلغ فاکتور به آن هدایت می‌شود.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.WebServiceUrl", "آدرس وب سرویس", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.WebServiceUrl.Hint", "آدرس وب سرویس بانک پاسارگاد برای ارتباط با درگاه پرداخت.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddSuccessNote", "ثبت نتیجه پرداخت موفقیت‌آمیز", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddSuccessNote.Hint", "نتیجه پرداخت موفقیت‌آمیز بعنوان یک یادداشت در فاکتور مشتری ثبت خواهد گردد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplaySuccessNoteToCustomer", "نمایش یادداشت پرداخت موفقیت‌آمیز به مشتری", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplaySuccessNoteToCustomer.Hint", "یادداشت نتیجه پرداخت موفقیت‌آمیز هم می‌تواند بعنوان تاریخچه فاکتور، فقط قابل مشاهده توسط ادمین سیستم باشد و هم اینکه مشتری به آن دسترسی داشته باشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddFailureNote", "ثبت نتیجه پرداخت ناموفق", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddFailureNote.Hint", "نتیجه پرداخت ناموفق بعنوان یک یادداشت در فاکتور مشتری ثبت خواهد گردد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplayFailureNoteToCustomer", "نمایش یادداشت پرداخت ناموفق به مشتری", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplayFailureNoteToCustomer.Hint", "یادداشت نتیجه پرداخت ناموفق هم می‌تواند بعنوان تاریخچه فاکتور، فقط قابل مشاهده توسط ادمین سیستم باشد و هم اینکه مشتری به آن دسترسی داشته باشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MaxAllowedNotes", "تعداد مجاز یادداشت فاکتور", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MaxAllowedNotes.Hint", "برای جلوگیری از حمله هکرها و ثبت بی‌رویه یادداشت به ازای پرداخت‌های ناموفق برای یک فاکتور، می‌توان سقف تعداد یادداشت تعیین کرد. در صورتی که تعداد یادداشت‌های فعلی فاکتور از این مقدار بیشتر باشد، یادداشت ناموفق جدیدی، برای این فاکتور در سیستم ثبت نخواهد گردید.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.PaymentMethodDescription", "پرداخت توسط درگاه بانک پاسارگاد", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.SuccessMessage", "صورتحساب با موفقیت پرداخت گردید.\r\nکد پیگیری : {0}", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.FailureMessage", "پردخت صورتحساب ناموفق بود.\r\nشرح خطا : {0}", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.OrderNotFound", "سفارش شماره #{0} پیدا نشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.CannotMarkOrderAsPaid", "امکان تغییر وضعیت سفارش شماره {0} به وضعیت پرداخت شده وجود ندارد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Settle.SuccessMessage", "مبلغ فاکتور با موفقیت به حساب بانکی، واریز گردید.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Settle.FailureMessage", "واریز مبلغ فاکتور به حساب بانکی، ناموفق بود.\r\nشرح خطا : {0}", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Payment.RedirectionTip", "برای تکمیل فرآیند خرید به درگاه بانک پاسارگاد متصل خواهید شد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Payment.Description", "بابت خرید فاکتور شماه {0}", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Payment.FailureMessage", "برقراری ارتباط با سرور بانک پاسارگاد مقدور نمی‌باشد.\r\nشرح خطا : {0}", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Reverse.SuccessMessage", "برگشت رسید دیجیتالی ({0}) موفقیت‌آمیز بود.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Reverse.FailureMessage", "برگشت رسید دیجیتالی ({0}) ناموفق بود.\r\nشرح خطا : {0}", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.0", "تراکنش با موفقیت انجام شد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.11", "شماره کارت نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.12", "موجودی کافی نیست.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.13", "رمز نادرست است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.14", "تعداد دفعات وارد کردن رمز بیش از حد مجاز است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.15", "کارت نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.16", "دفعات برداشت وجه بیش از حد مجاز است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.17", "کاربر از انجام تراکنش منصرف شده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.18", "تاریخ انقضای کارت گذشته است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.19", "مبلغ برداشت وجه بیش از حد مجاز است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.21", "پذیرنده نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.23", "خطای امنیتی رخ داده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.24", "اطلاعات کاربری پذیرنده نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.25", "مبلغ نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.31", "پاسخ نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.32", "فرمت اطلاعات وارد شده صحیح نمی‌باشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.33", "حساب نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.34", "خطای سیستمی.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.35", "تاریخ نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.41", "شماره درخواست تکراری است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.42", "تراکنش Sale یافت نشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.43", "قبلاً درخواست Verify داده شده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.44", "درخواست Verfiy یافت نشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.45", "تراکنش Settle شده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.46", "تراکنش Settle نشده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.47", "تراکنش Settle یافت نشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.48", "تراکنش Reverse شده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.49", "تراکنش Refund یافت نشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.51", "تراکنش تکراری است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.54", "تراکنش مرجع موجود نیست.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.55", "تراکنش نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.61", "خطا در واریز.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.111", "صادر کننده کارت نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.112", "خطای سوییچ صادر کننده کارت.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.113", "پاسخی از صادر کننده کارت دریافت نشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.114", "دارنده کارت مجاز به انجام این تراکنش نیست.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.412", "شناسه قبض نادرست است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.413", "شناسه پرداخت نادرست است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.414", "سازمان صادر کننده قبض نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.415", "زمان جلسه کاری به پایان رسیده است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.416", "خطا در ثبت اطلاعات.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.417", "شناسه پرداخت کننده نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.418", "اشکال در تعریف اطلاعات مشتری.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.419", "تعداد دفعات ورود اطلاعات از حد مجاز گذشته است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.421", "IP نامعتبر است.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.InvalidResult", "نتیجه دریافتی معتبر نمی‌باشد.", null);
            await this._localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.ConfirmTransaction", "Confirm Transaction");

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await this._settingService.DeleteSettingAsync<BankPasargadPaymentSettings>();
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFeePercentage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFeePercentage.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFee");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AdditionalFee.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.ShippableProductRequired");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.ShippableProductRequired.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PrivateKeyText");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PrivateKeyText.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MerchantCode");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MerchantCode.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.TerminalCode");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.TerminalCode.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PayerId");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PayerId.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PaymentUrl");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.PaymentUrl.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.WebServiceUrl");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.WebServiceUrl.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddSuccessNote");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddSuccessNote.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplaySuccessNoteToCustomer");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplaySuccessNoteToCustomer.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddFailureNote");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.AddFailureNote.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplayFailureNoteToCustomer");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.DisplayFailureNoteToCustomer.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MaxAllowedNotes");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Fields.MaxAllowedNotes.Hint");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.PaymentMethodDescription");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.SuccessMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.FailureMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Verify.OrderNotFound");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankSaman.Verify.CannotMarkOrderAsPaid");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Settle.SuccessMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Settle.FailureMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Payment.RedirectionTip");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Payment.Description");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.Payment.FailureMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankSaman.Reverse.SuccessMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankSaman.Reverse.FailureMessage");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.0");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.11");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.12");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.13");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.14");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.15");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.16");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.17");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.18");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.19");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.21");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.23");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.24");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.25");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.31");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.32");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.33");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.34");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.35");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.41");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.42");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.43");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.44");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.45");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.46");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.47");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.48");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.49");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.51");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.54");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.55");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.61");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.111");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.112");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.113");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.114");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.412");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.413");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.414");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.415");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.416");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.417");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.418");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.419");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.421");
            await this._localizationService.DeleteLocaleResourceAsync("Plugins.Payments.BankPasargad.StatusCode.InvalidResult");
            await base.UninstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            if (this.PluginDescriptor.Author != "http://nopFarsi.IR team")
                throw new Exception();
            return this._webHelper.GetStoreLocation(null) + "Admin/BankPasargad/Configure";
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

        public Type GetPublicViewComponent()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await this._localizationService.GetResourceAsync("Plugins.Payments.BankPasargad.PaymentMethodDescription");
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
                return RecurringPaymentType.NotSupported;
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



        private const string GenericAttributeRefId = "BankPasargadRefId";

        private const string BankPrivateKey = "<RSAKeyValue><Modulus>zFb5W0Xw1JtN0QCqDXSfhIurX1PciqvvYyeIu/veaHYLPJVCLHAkdwAIj6qlLerjm4iXOizxxaJkfXzAp8RfTtpJ3PqlBV/rI0r7cZWBUs6SlvE8RPwq4puCJTVIWI5YKYg6FiSClMJfiJkGTnmN4wIfnLFDKkHUKihv8MSZiz0e/DVeyykRg9Dwb/HLRPVY2TrFVKZfjJKWf7TrXYt7JeIZqnzDgSAN8LtFLHXB+75CMWJJktOX9LwjNTKl2LaLuLe/ivnToX9Bm/5VaWyB4eOujIirmbXU0IRDK8eZnPbp3f+NePlteq6gvbkG54ShYIes9+8G+YpfG9bJ8J3jpQ==</Modulus><Exponent>AQAB</Exponent><P>5eqWLxFU7b2gOnpoXunrdPwDlvVeulV8lntShlSMp8DXGWxOeAEVAtcD6mPisbfp7oYSoCcxdZ9jvEMg3maV2N40n/DPv+BFuJ450fh0bZ+0vWp5z6mV4s15eeWCvzzBumjoge5mKvtZN7quWSX/okjtx8Owe38qgWaZTumhWtc=</P><Q>44WSNSOFEWlQNnAEp3Rf6RDUKNecuwqT3mdxlyMX+XPrluu3nJAnEssdepR2ZdUKLSaO44llK6WGJHMEEmgPATCpenAkG5x6U1FOralzeSbcLrbYBUUK4g8c2rewuFRiBOM3LdBELykI8vrZBI7ofMRPvVmZryftWLU9WPHcgeM=</Q><DP>YDSD+Vf4QK7LQAz+ZVDebIoTenRb2f5xbopV4s24OW2w0/uE7iQNxa1w/wAi2JiLc3p4irk7sesrEZjJRSaJ50A7pv1eAPz4T6MtsJWVKlXcjDHknKdqh4fRH9eQmTL0IWtKdyvpZmzcTTx5PeZvjFmV3bQdwd9ZSuJCnkmWces=</DP><DQ>hOQ0pNMNW0Ee99zv6vsfNht9QCnHwrwH6GSVRUjbLPnXRjyB73GHUQPQQRPMH+iSXBLkbOSIcjjpJAsF+tZKxVAt/XTYskPwcw/Pm8GYz8+x1Zs4i8Nq+uUG3n1iPBgVdHNefGTkHoOiVYHNFkt+YbmsNZXLHTJ+AE1D+tqyCC8=</DQ><InverseQ>Rt/uO6PgN/+kqU9B8x5En5pWdjUOpmxeCuhyKjdg2JwWVxnICYKvGOe6eIBUIbw5/Bj1OWIDxxxLXkEZPKfiS9SxF9gUujTEBA+chxD6LJmYOLZZ5FVgFxxt8JrGWOvXtemthP9btbevpdYpxrZigTfbaWLcd0EgTeCr26Dn0Zc=</InverseQ><D>kXLDMaXaI+ub+04GvWcq+JTWWhd+XzK9Rg3o9S32BNMpcmkn3ivAuPfMID8uLj4I3zfdtSoyuiO6ddwr4f8UfDNC6kl61xWMcOW5jwMFN6UUkN6LsOrxt7VLcSK5KVfBB2cROZupWyv0Bm4uIAPD7Dchu04t6dFLrOp1q6K1THbikLrYT89e2b9u5eI4qZuKiptdX1FYOHA0btq1JJUqwshCFtGl+dmCcKh0/4L8CbpYqpvw/vW3/jSi4n8mQ1GO7cCfIQd4tudn1CdYERmhslQ5UUmKieKHJXFSIijtBw+7OHRqBmis8fug9Hos3nqrBZA+SBKbRRCA/RwbJoBSjQ==</D></RSAKeyValue>";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly BankPasargadPaymentSettings _bankPasargadPaymentSettings;

        private readonly ILocalizationService _localizationService;

        private readonly ISettingService _settingService;

        private readonly IWebHelper _webHelper;

        private readonly IGenericAttributeService _genericAttributeService;

        private readonly IPaymentService _paymentService;

        private readonly IOrderService _orderService;

        private readonly IOrderProcessingService _orderProcessingService;

        private readonly IShoppingCartService _shoppingCartService;
       
    }
}