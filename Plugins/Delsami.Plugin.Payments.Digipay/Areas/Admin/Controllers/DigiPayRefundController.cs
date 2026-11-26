using DocumentFormat.OpenXml.EMMA;
using FluentMigrator.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Delsami.Plugin.Misc.Core.Helpers;
using Delsami.Plugin.Misc.Core.Models.ApiCaller;
using Delsami.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund;
using Delsami.Plugin.Payments.Digipay.Domain;
using Delsami.Plugin.Payments.Digipay.Infrastructure.Permissions;
using Delsami.Plugin.Payments.Digipay.Models.DigiPays;
using Delsami.Plugin.Payments.Digipay.Services;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Digipay.Areas.Admin.Controllers
{
	[AuthorizeAdmin]
	[Area(AreaNames.Admin)]
	public class DigiPayRefundController : BasePluginController
	{
		#region Fields

		private readonly ILocalizationService _localizationService;
		private readonly INotificationService _notificationService;
		private readonly IPermissionService _permissionService;
		private readonly DigiPaySettings _digiPaySettings;
		private readonly IDigiPayService _digiPayService;
		private readonly ILogger _logger;
		private readonly IWorkContext _workContext;

		#endregion

		#region Ctor

		public DigiPayRefundController(ILocalizationService localizationService,
								 INotificationService notificationService,
								 IPermissionService permissionService,
								 DigiPaySettings digiPaySettings,
								 IDigiPayService digiPayService,
								 ILogger logger,
								 IWorkContext workContext)
		{
			_localizationService = localizationService;
			_notificationService = notificationService;
			_permissionService = permissionService;
			_digiPaySettings = digiPaySettings;
			_digiPayService = digiPayService;
			_logger = logger;
			_workContext = workContext;
		}

		#endregion

		#region Methods

		public async Task<IActionResult> RefundList()
		{
			if (!await _permissionService.AuthorizeAsync(DigiPayRefundPermission.DigiPayRefund))
				return AccessDeniedView();

			var searchModel = new DigiPayRefundSearchModel();

			searchModel.SetGridPageSize(10);

			return View("~/Plugins/Delsami.Payments.Digipay/Areas/Admin/Views/DigiPay/RefundList.cshtml", searchModel);
		}

		[HttpPost]
		public async Task<IActionResult> RefundList(DigiPayRefundSearchModel refundSearchModel)
		{
			if (!await _permissionService.AuthorizeAsync(DigiPayRefundPermission.DigiPayRefund))
				return await AccessDeniedDataTablesJson();

			var model = await PrepareDigiPayRefundListModel(refundSearchModel);

			return Json(model);
		}


		[HttpPost]
		public async Task<IActionResult> CreateRefundRecord(DigiPayRefundModel model)
		{
			if (!ModelState.IsValid)
			{
				var errorMessage = string.Join(" | ", ModelState.Values
										 .SelectMany(v => v.Errors)
										 .Select(e => e.ErrorMessage));

				_notificationService.ErrorNotification(errorMessage);
				return RedirectToAction("RefundList");
			}

			try
			{
				if (!await _permissionService.AuthorizeAsync(DigiPayRefundPermission.DigiPayRefund))
					return AccessDeniedView();

				var refundRequest = new RefundRequestModel
				{
					providerId = Guid.NewGuid().ToString(),
					amount = model.Amount,
					saleTrackingCode = model.PaymentTrackingCode,
				};

				var (isSuccess, token) = await _digiPayService.GetTokenAsync();

				string errorMessage = isSuccess ? "" : "خطا در دریافت توکن";

				(bool CallIsSuccess, ApiResponse<RefundResultModel> RefundResponse) result = (false, null);
				if (isSuccess)
				{
					result = await _digiPayService.RefundAsync(refundRequest, token.access_token, (int)model.PaymentType);

					if (!result.CallIsSuccess && !string.IsNullOrEmpty(result.RefundResponse?.ErrorMessage))
					{
						var jo = JObject.Parse(result.RefundResponse.ErrorMessage);
						errorMessage = jo["result"]?.ToObject<ResultModel>()?.message;
					}
				}

				await InsertDigiPayRefundRecordAsync(result.CallIsSuccess,result.RefundResponse?.Data,errorMessage,model);

				if (result.CallIsSuccess)
					_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.SuccessfullyRefund"));
				else
					_notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.ErrorInRefund"));

				return RedirectToAction("RefundList");

			}
			catch (Exception ex)
			{
				await _logger.ErrorAsync($"DigiPay Payment Refund with tracking Code({model.RefundTrackingCode}) Was Failed Due to: {JsonConvert.SerializeObject(ex.Message)}", ex);
				_notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.ErrorInRefund"));

				await InsertDigiPayRefundRecordAsync(false, null, ex.Message, model);

				return RedirectToAction("RefundList");
			}

		}

		#endregion

		#region Private Methods
		private async Task<DigiPayRefundRecordListModel> PrepareDigiPayRefundListModel(DigiPayRefundSearchModel searchModel)
		{
			if (searchModel == null)
				throw new ArgumentNullException(nameof(searchModel));

			var digiPayRefundRecords = await _digiPayService.GetAllDigiPayRefundRecordsAsync(searchModel.TrackingCodeKeyWord, searchModel.Page - 1, searchModel.PageSize);

			var model = new DigiPayRefundRecordListModel().PrepareToGrid(searchModel, digiPayRefundRecords, () =>
			{
				return digiPayRefundRecords.Select(r => new DigiPayRefundModel
				{
					Amount = r.Amount.ToString(),
					Message = r.Message,
					RefundDateTimeOnUtcStr = DateTimeHelper.GetPersianStrFromDateTime(r.RefundDateTimeOnUtc.ToLocalTime(), true),
					Status = r.Status,
					RefundTrackingCode = r.RefundTrackingCode,
					PaymentTrackingCode = r.PaymentTrackingCode,
					PaymentTypeStr = r.PaymentType.ToString(),
					Customer = r.Customer,
				});
			});

			return model;
		}


		private async Task InsertDigiPayRefundRecordAsync(bool isSuccess, RefundResultModel refundResponse, string errorMessage, DigiPayRefundModel model)
		{
			var customer = await _workContext.GetCurrentCustomerAsync();

			var entity = new DigiPayRefundRecord
			{
				Customer = customer.FirstName + " " + customer.LastName,
				RefundDateTimeOnUtc = DateTime.UtcNow,
				PaymentTrackingCode =model.PaymentTrackingCode,
				RefundTrackingCode = isSuccess ? refundResponse.trackingCode : null,
				Status = isSuccess ? refundResponse.result.status == 0 : false,
				Message = isSuccess ? refundResponse.result.message : errorMessage,
				PaymentType = model.PaymentType,
				Amount = model.Amount ?? "0"
			};

			await _digiPayService.InsertDigiPayRefundRecordAsync(entity);
		}
		#endregion
	}
}
