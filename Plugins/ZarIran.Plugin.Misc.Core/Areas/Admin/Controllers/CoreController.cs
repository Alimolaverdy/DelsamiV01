using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using ZarIran.Plugin.Misc.Core.Areas.Admin.Models;
using System.Threading.Tasks;
using Nop.Services.Customers;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ZarIran.Plugin.Misc.Core.Areas.Admin.Controllers
{

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CoreController : BasePluginController
    {

        #region Fields

        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public CoreController(ISettingService settingService,
                      INotificationService notificationService,
                      ICustomerService customerService,
                      ILocalizationService localizationService)
        {
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            var coreSettings = await _settingService.LoadSettingAsync<CoreSettings>();

            var model = new ConfigurationModel
            {
                ManualGoldPrice = coreSettings.ManualGoldPrice,
                UseManualGoldPrice = coreSettings.UseManualGoldPrice,
                GoldPriceAdjustment = coreSettings.GoldPriceAdjustment,
                AllowGoldPriceAdjustment = coreSettings.AllowPriceAdjustment,
                Tax = coreSettings.Tax,
                NationalCodeAttributeId = coreSettings.NationalCodeAttributeId,
                AllProductsCategoryId = coreSettings.AllProductsCategoryId,
                TradeDiscountId = coreSettings.TradeDiscountId,
                CoWorkerRoleId = coreSettings.CoWorkerRoleId,
                AvailableCustomerRoleIds = (await _customerService.GetAllCustomerRolesAsync())
                    .Select(role => new SelectListItem
                    {
                        Text = role.Name,
                        Value = role.Id.ToString()
                    }).ToList()
            };


            return View("~/Plugins/ZarIran.Misc.Core/Areas/Admin/Views/Core/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var coreSettings = await _settingService.LoadSettingAsync<CoreSettings>();

            coreSettings.ManualGoldPrice = model.ManualGoldPrice;
            coreSettings.Tax = model.Tax;
            coreSettings.UseManualGoldPrice = model.UseManualGoldPrice;
            coreSettings.AllowPriceAdjustment = model.AllowGoldPriceAdjustment;
            coreSettings.GoldPriceAdjustment = model.GoldPriceAdjustment;
            coreSettings.NationalCodeAttributeId = model.NationalCodeAttributeId;
            coreSettings.AllProductsCategoryId = model.AllProductsCategoryId;
            coreSettings.TradeDiscountId = model.TradeDiscountId;
            coreSettings.CoWorkerRoleId = model.CoWorkerRoleId;



            await _settingService.SaveSettingAsync(coreSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion

    }
}
