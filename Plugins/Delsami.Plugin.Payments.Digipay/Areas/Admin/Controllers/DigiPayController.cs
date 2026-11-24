using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZarIran.Plugin.Payments.Digipay.Areas.Admin.Models;
using ZarIran.Plugin.Payments.Digipay.Infrastructure.Permissions;
using ZarIran.Plugin.Payments.Digipay.Models.DigiPays;
using ZarIran.Plugin.Payments.Digipay.Services;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Digipay.Areas.Admin.Controllers
{
	[AuthorizeAdmin]
	[Area(AreaNames.Admin)]
	public class DigiPayController : BasePluginController
	{
		#region Fields

		private readonly ILanguageService _languageService;
		private readonly ILocalizationService _localizationService;
		private readonly INotificationService _notificationService;
		private readonly IPermissionService _permissionService;
		private readonly ISettingService _settingService;
		private readonly IStoreContext _storeContext;
		private readonly IDigiPayService _digiPayService;
		private readonly ILogger _logger;
		private readonly IWorkContext _workContext;

		#endregion

		#region Ctor

		public DigiPayController(ILanguageService languageService,
									ILocalizationService localizationService,
									INotificationService notificationService,
									IPermissionService permissionService,
									ISettingService settingService,
									IStoreContext storeContext,
									IDigiPayService digiPayService,
									ILogger logger,
									IWorkContext workContext)
		{
			_languageService = languageService;
			_localizationService = localizationService;
			_notificationService = notificationService;
			_permissionService = permissionService;
			_settingService = settingService;
			_storeContext = storeContext;
			_digiPayService = digiPayService;
			_logger = logger;
			_workContext = workContext;
		}

		#endregion

		#region Methods

		public async Task<IActionResult> Configure()
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
				return AccessDeniedView();

			//load settings for a chosen store scope
			var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			var checkMoneyOrderPaymentSettings = _settingService.LoadSetting<DigiPaySettings>(storeScope);

			var model = new ConfigurationModel
			{
				BaseUrl = checkMoneyOrderPaymentSettings.BaseUrl,
				DigiPayVersion = checkMoneyOrderPaymentSettings.DigiPayVersion,
				Username = checkMoneyOrderPaymentSettings.Username,
				Password = checkMoneyOrderPaymentSettings.Password,
				ClientId = checkMoneyOrderPaymentSettings.ClientId,
				ClientSecret = checkMoneyOrderPaymentSettings.ClientSecret,
				GrantType = checkMoneyOrderPaymentSettings.GrantType
			};

			return View("~/Plugins/ZarIran.Payments.Digipay/Areas/Admin/Views/DigiPay/Configure.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Configure(ConfigurationModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
				return AccessDeniedView();

			if (!ModelState.IsValid)
				return await Configure();

			//load settings for a chosen store scope
			var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			var checkMoneyOrderPaymentSettings = _settingService.LoadSetting<DigiPaySettings>(storeScope);

			//save settings
			checkMoneyOrderPaymentSettings.BaseUrl = model.BaseUrl;
			checkMoneyOrderPaymentSettings.DigiPayVersion = model.DigiPayVersion;
			checkMoneyOrderPaymentSettings.Username = model.Username;
			checkMoneyOrderPaymentSettings.Password = model.Password;
			checkMoneyOrderPaymentSettings.GrantType = model.GrantType;
			checkMoneyOrderPaymentSettings.ClientId = model.ClientId;
			checkMoneyOrderPaymentSettings.ClientSecret = model.ClientSecret;

			_settingService.SaveSetting(checkMoneyOrderPaymentSettings);
			//now clear settings cache
			_settingService.ClearCache();

			_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

			return await Configure();
		}

		#endregion
	}
}
