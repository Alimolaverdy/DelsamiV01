using ZarIran.Plugin.Misc.Core.Models.ApiCaller;
using ZarIran.Plugin.Payment.Core.Services;
using ZarIran.Plugin.Payments.Digipay.Domain;
using ZarIran.Plugin.Payments.Digipay.Models.DigiPays;
using Nop.Core;
using Nop.Data;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Digipay.Services
{
	public class DigiPayService : IDigiPayService
	{
		#region Fields

		private readonly DigiPaySettings _digiPayBPGSettings;
		private readonly ILogger _logger;
		private readonly IPaymentApiCaller _paymentApiCaller;
		private readonly IRepository<DigiPayRefundRecord> _digiPayRefundRecordRepository;

		#endregion

		#region Ctor

		public DigiPayService(DigiPaySettings digiPayBPGSettings, IPaymentApiCaller paymentApiCaller, ILogger logger, IRepository<DigiPayRefundRecord> digiPayRefundRecordRepository)
		{
			_digiPayBPGSettings = digiPayBPGSettings;
			_logger = logger;
			_paymentApiCaller = paymentApiCaller;
			_digiPayRefundRecordRepository = digiPayRefundRecordRepository;
		}

		#endregion

		#region Methods

		public async Task<(bool CallIsSuccess, GetTokenResponseModel TokenResult)> GetTokenAsync()
		{
			var url = _digiPayBPGSettings.BaseUrl + "/oauth/token";

			var authorizationValue = $"{_digiPayBPGSettings.ClientId}:{_digiPayBPGSettings.ClientSecret}";
			var authorizationValueBytesArray = Encoding.UTF8.GetBytes(authorizationValue);
			var authorization = Convert.ToBase64String(authorizationValueBytesArray);

			try
			{
				using (var client = new HttpClient())
				{
					//client.DefaultRequestHeaders.Add("authorization", $"Basic {authorization}");
					var headers = new Dictionary<string, string>
					{
						{ "authorization", $"Basic {authorization}" }
					};

					var formData = new Dictionary<string, string>
					{
						{ "username", _digiPayBPGSettings.Username },
						{ "password", _digiPayBPGSettings.Password },
						{ "grant_type", _digiPayBPGSettings.GrantType }
					};

					var response = await _paymentApiCaller.PostAsync<Dictionary<string, string>, GetTokenResponseModel>(url,
																												 formData,
																												 headers: headers,
																												 contentType: "application/x-www-form-urlencoded");

					if (!response.IsSuccess)
					{
						await _logger.ErrorAsync($"Error in digipay get token:{response.ErrorMessage}");
						return (false, null);
					}

					return (true, response.Data);
				}
			}
			catch (Exception ex)
			{
				await _logger.ErrorAsync(ex.Message, ex);

				return (false, null);
			}
		}

		public async Task<(bool CallIsSuccess, CreateTicketResponseModel CreateTicketResult)> CreateTicketAsync(CreateTicketModel createTicket, string token)
		{
			var url = _digiPayBPGSettings.BaseUrl + "/tickets/business?type=11";

			try
			{
				var headers = new Dictionary<string, string>
				{
					{ "Agent", "WEB" },
					{ "Digipay-Version", _digiPayBPGSettings.DigiPayVersion }
				};

				var response = await _paymentApiCaller.PostAsync<CreateTicketModel, CreateTicketResponseModel>(
					url,
					createTicket,
					bearerToken: token,
					headers: headers);

				if (!response.IsSuccess)
				{
					await _logger.ErrorAsync($"Error in digipay create ticket:{response.ErrorMessage}");

					return (false, null);
				}

				return (true, response.Data);
			}
			catch (Exception ex)
			{
				await _logger.ErrorAsync(ex.Message, ex);

				return (false, null);
			}
		}

		public async Task<(bool CallIsSuccess, VerifyResponseModel VerifyResponse)> VerifyAsync(VerifyRequestModel model, string token, int type)
		{
			var url = _digiPayBPGSettings.BaseUrl + $"/purchases/verify?type={type}";

			try
			{
				var response = await _paymentApiCaller.PostAsync<VerifyRequestModel, VerifyResponseModel>(
					url,
					model,
					bearerToken: token);

				if (!response.IsSuccess)
				{
					await _logger.ErrorAsync($"Error in digipay verify:{response.ErrorMessage}");

					return (false, null);
				}

				return (true, response.Data);
			}
			catch (Exception ex)
			{
				await _logger.InformationAsync($"digipay exception in verify service: {ex.Message}", ex);
				await _logger.ErrorAsync($"digipay exception in verify service: {ex.Message}", ex);

				return (false, null);
			}
		}

		public async Task<(bool CallIsSuccess, DeliverResponseModel DeliverResponse)> DeliverAsync(
		DeliverRequestModel model, string token, int type)
		{
			var url = _digiPayBPGSettings.BaseUrl + $"/purchases/deliver?type={type}";

			try
			{
				var response = await _paymentApiCaller.PostAsync<DeliverRequestModel, DeliverResponseModel>(
					url,
					model,
					bearerToken: token);

				if (!response.IsSuccess)
				{
					await _logger.ErrorAsync($"Error in digipay deliver:{response.ErrorMessage}");

					return (false, null);
				}

				return (true, response.Data);
			}
			catch (Exception ex)
			{
				await _logger.InformationAsync($"digipay exception in deliver service: {ex.Message}", ex);
				await _logger.ErrorAsync($"digipay exception in deliver service: {ex.Message}", ex);

				return (false, null);
			}
		}

		public async Task<(bool CallIsSuccess, ApiResponse<RefundResultModel> RefundResponse)> RefundAsync(RefundRequestModel model, string token, int type)
		{
			var url = _digiPayBPGSettings.BaseUrl + $"/refunds?type={type}";

			try
			{
				var response = await _paymentApiCaller.PostAsync<RefundRequestModel, RefundResultModel>(
					url,
					model,
					bearerToken: token);

				if (!response.IsSuccess)
				{
					await _logger.ErrorAsync($"Error in digipay refund:{response.ErrorMessage}");

					return (false, response);
				}

				return (true, response);
			}
			catch (Exception ex)
			{
				await _logger.ErrorAsync(ex.Message, ex);
				return (false, null);
			}
		}

		public async Task InsertDigiPayRefundRecordAsync(DigiPayRefundRecord model)
		{
			await _digiPayRefundRecordRepository.InsertAsync(model);
		}

		public async Task<IPagedList<DigiPayRefundRecord>> GetAllDigiPayRefundRecordsAsync(string trackingCode, int pageIndex = 0,int pageSize = int.MaxValue)
		{
			var query=_digiPayRefundRecordRepository.Table;

			if (!string.IsNullOrEmpty(trackingCode?.Trim()))
				query = query.Where(a => a.PaymentTrackingCode.Contains(trackingCode));

			return await query.OrderByDescending(a=>a.RefundDateTimeOnUtc).ToPagedListAsync(pageIndex, pageSize);
		}




		#endregion
	}
}
