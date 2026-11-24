using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using NopFarsi.Payments.IranKish.Models;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.IranKish.Controller
{
    public class IkcNopFarsiController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly INotificationService _notificationService;
        private readonly IStoreContext _storeContext;

        public IkcNopFarsiController(
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            PaymentSettings paymentSettings,
            ILogger logger,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            INotificationService notificationService,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._logger = logger;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._notificationService = notificationService;
            this._storeContext = storeContext;
        }

        public async Task<IActionResult> Pay(string ResNum, string MID, string Amount, string RedirectURL)
        {
            var mm = new shaparak.ikc.service.TokensClient();

            int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            IkcSettings ikcSettings = this._settingService.LoadSetting<IkcSettings>(activeStoreScopeConfiguration);

            string token;

            if (!string.IsNullOrEmpty(ikcSettings.Apportionment))
            {
                long aa = int.Parse(Amount);
                var pp = ikcSettings.Apportionment.Split(',');
                string ExtraParam4 = "";
                for (var i = 0; i < pp.Length; i++)
                {
                    ExtraParam4 += (i + "," + (aa * int.Parse(pp[i]) / 100) + ";");
                }

                var tokenResp = mm.MakeSpecialTokenAsync(Amount, MID, ResNum, ResNum, null, RedirectURL, "خرید فروشگاه با تسهیم", null, null, "11", ExtraParam4);
                token = tokenResp.Result.token;
            }
            else
            {
                var tokenResp = mm
                    .MakeTokenAsync(Amount, MID, ResNum, ResNum, "", RedirectURL, "خرید فروشگاه");
                token = tokenResp.Result.token;
            }


            //==================================================

            NameValueCollection datacollection = new NameValueCollection();

            datacollection.Add("token", token);
            datacollection.Add("ResNum", ResNum);

            datacollection.Add("merchantId", MID);

            datacollection.Add("PaymentId", ResNum);

            string text31 = PreparePOSTFormSaman("https://ikc.shaparak.ir/TPayment/Payment/Index", datacollection);//;
            //return View(text31);
            return base.View("~/Plugins/NopFarsi.Payments.IranKish/Views/Pay.cshtml", model: text31);

        }

        private String PreparePOSTFormSaman(string url, NameValueCollection data)
        {
            //Set a name for the form
            string formID = "PostForm";

            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            //strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
            foreach (string key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
            }
            //strForm.Append("</form>");

            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            //Return the form and the script concatenated. (The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }

        public async Task<IActionResult> Verify(string token, string merchantId, string resultCode, string paymentId, string referenceId)
        {
            if (string.IsNullOrEmpty(paymentId))
            {
                return base.RedirectToAction("Index", "Home", new
                {
                    area = ""
                });

            }
            if (resultCode.Equals("100"))
            {
                // You should send this parameter to verifier service 
                var sha1Key = "22338240992352910814917221751200141041845518824222260";

                var VerifyService = new shaparak.ikc.VerifyClient();

                var result = VerifyService.KicccPaymentsVerificationAsync(token, merchantId, referenceId, sha1Key);
                result.Wait();

                if (result.Result > 0)
                {
                    int orderId = int.Parse(paymentId);
                    Order orderById = await this._orderService.GetOrderByIdAsync(orderId);
                    orderById.AuthorizationTransactionResult = (string.Format("[ResCode = {0}]", result.Result));
                    orderById.AuthorizationTransactionId = (referenceId);
                    await this._orderService.UpdateOrderAsync(orderById);
                    await this._orderProcessingService.MarkOrderAsPaidAsync(orderById);

                    return base.RedirectToRoute("CheckoutCompleted", new
                    {
                        orderId
                    });
                }
            }
            else
            {
                EngineContext.Current.Resolve<ILogger>().Information("Error return Url Sep: " + resultCode + " Order Id:" + paymentId, null, null);
                return this.Redirect("/orderdetails/" + paymentId.ToString());
                //return base.RedirectToRoute("HomePage");
            }

            return base.RedirectToRoute("HomePage");


        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpGet]
        public async Task<IActionResult> Configure()
        {
            int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            IkcSettings sepPaymentSettings = this._settingService.LoadSetting<IkcSettings>(activeStoreScopeConfiguration);
            ConfigurationModel configurationModel = new ConfigurationModel
            {
                MerchantId = sepPaymentSettings.MerchantId,
                IsTomanForStore = sepPaymentSettings.IsToman,
                DisablePaymentInfoForStore = sepPaymentSettings.DisablePaymentInfo,
                ActiveStoreScopeConfiguration = activeStoreScopeConfiguration,
            };

            if (activeStoreScopeConfiguration > 0)
            {
                configurationModel.MerchantIdOverrideForStore = this._settingService.SettingExists<IkcSettings, string>(sepPaymentSettings, (IkcSettings x) => x.MerchantId, activeStoreScopeConfiguration);
                configurationModel.IsTomanForStore = this._settingService.SettingExists<IkcSettings, bool>(sepPaymentSettings, (IkcSettings x) => x.IsToman, activeStoreScopeConfiguration);
                configurationModel.DisablePaymentInfo = this._settingService.SettingExists<IkcSettings, bool>(sepPaymentSettings, (IkcSettings x) => x.DisablePaymentInfo, activeStoreScopeConfiguration);
                configurationModel.ApportionmentForStore = this._settingService.SettingExists<IkcSettings, string>(sepPaymentSettings, (IkcSettings x) => x.Apportionment, activeStoreScopeConfiguration);
            }
            return base.View("~/Plugins/NopFarsi.Payments.IranKish/Views/Configure.cshtml", configurationModel);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!base.ModelState.IsValid)
            {
                return await this.Configure();
            }
            int activeStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            IkcSettings zarinpalPaymentSettings = this._settingService.LoadSetting<IkcSettings>(activeStoreScopeConfiguration);

            zarinpalPaymentSettings.MerchantId = model.MerchantId;

            zarinpalPaymentSettings.IsToman = model.IsTomanForStore;
            zarinpalPaymentSettings.DisablePaymentInfo = model.DisablePaymentInfoForStore;

            await this._settingService.SaveSettingOverridablePerStoreAsync<IkcSettings, string>(zarinpalPaymentSettings, (IkcSettings x) => x.MerchantId, model.MerchantIdOverrideForStore, activeStoreScopeConfiguration, false);
            await this._settingService.SaveSettingOverridablePerStoreAsync<IkcSettings, bool>(zarinpalPaymentSettings, (IkcSettings x) => x.IsToman, model.IsTomanForStore, activeStoreScopeConfiguration, false);
            await this._settingService.SaveSettingOverridablePerStoreAsync<IkcSettings, bool>(zarinpalPaymentSettings, (IkcSettings x) => x.DisablePaymentInfo, model.DisablePaymentInfoForStore, activeStoreScopeConfiguration, false);
            this._settingService.ClearCache();
            this._notificationService.SuccessNotification("با موفقیت ذخیره شد. تولید شده توسط ناپ فارسی . www.nopfarsi.ir");
            return base.View("~/Plugins/NopFarsi.Payments.IranKish/Views/Configure.cshtml", model);

        }
    }
}
