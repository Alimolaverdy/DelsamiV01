using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Delsami.Plugin.Payments.Barook.Areas.Admin.Models;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Barook.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
public class BarookController : BasePluginController
{
	#region Fields

	private readonly ILocalizationService _localizationService;
	private readonly INotificationService _notificationService;
	private readonly IPermissionService _permissionService;
	private readonly ISettingService _settingService;
	private readonly IStoreContext _storeContext;

	#endregion

	#region Ctor

	public BarookController(ILocalizationService localizationService, INotificationService notificationService, IPermissionService permissionService,
						  ISettingService settingService, IStoreContext storeContext)
	{
		_localizationService = localizationService;
		_notificationService = notificationService;
		_permissionService = permissionService;
		_settingService = settingService;
		_storeContext = storeContext;
	}

	#endregion

	#region Methods

	public async Task<IActionResult> Configure()
	{
		if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
			return AccessDeniedView();

		//load settings for a chosen store scope
		var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		var BarookSettings = _settingService.LoadSetting<BarookSettings>(storeScope);


		var model = new ConfigurationModel
		{
			BaseUrl = BarookSettings.BaseUrl,
			TerminalCode = BarookSettings.TerminalCode,
			TerminalPassword = BarookSettings.TerminalPassword,
			businessServiceSlug = BarookSettings.businessServiceSlug,
			paymentDailyCount = BarookSettings.paymentDailyCount,
		};

		model.AvaiablepaymentDailyCount = new()
		{
			new SelectListItem { Text = "30", Value = "30" },
			new SelectListItem { Text = "60", Value = "60" },
			new SelectListItem { Text = "90", Value = "90" }
		};

		return View("~/Plugins/Delsami.Payments.Barook/Areas/Admin/Views/Barook/Configure.cshtml", model);
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
		var BarookSettings = _settingService.LoadSetting<BarookSettings>(storeScope);

		//save settings
		BarookSettings.BaseUrl = model.BaseUrl;
		BarookSettings.TerminalPassword = model.TerminalPassword;
		BarookSettings.TerminalCode = model.TerminalCode;
		BarookSettings.businessServiceSlug = model.businessServiceSlug;
		BarookSettings.paymentDailyCount = model.paymentDailyCount;


		_settingService.SaveSetting(BarookSettings);
		//now clear settings cache
		_settingService.ClearCache();

		_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

		return await Configure();
	}


	#endregion
}
