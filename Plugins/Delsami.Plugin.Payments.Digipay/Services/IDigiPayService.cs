using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ZarIran.Plugin.Misc.Core.Models.ApiCaller;
using ZarIran.Plugin.Payments.Digipay.Domain;
using ZarIran.Plugin.Payments.Digipay.Models.DigiPays;
using Nop.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Digipay.Services
{
    public interface IDigiPayService
    {
        Task<(bool CallIsSuccess, GetTokenResponseModel TokenResult)> GetTokenAsync();

        Task<(bool CallIsSuccess, CreateTicketResponseModel CreateTicketResult)> CreateTicketAsync(CreateTicketModel createTicket, string token);

        Task<(bool CallIsSuccess, VerifyResponseModel VerifyResponse)> VerifyAsync(VerifyRequestModel model, string token, int type);

        Task<(bool CallIsSuccess, DeliverResponseModel DeliverResponse)> DeliverAsync(DeliverRequestModel model, string token, int type);

        Task<(bool CallIsSuccess, ApiResponse<RefundResultModel> RefundResponse)> RefundAsync(RefundRequestModel model, string token, int type);



		Task InsertDigiPayRefundRecordAsync(DigiPayRefundRecord model);

        Task<IPagedList<DigiPayRefundRecord>> GetAllDigiPayRefundRecordsAsync(string trackingCode, int pageIndex = 0, int pageSize = int.MaxValue);

    }
}
