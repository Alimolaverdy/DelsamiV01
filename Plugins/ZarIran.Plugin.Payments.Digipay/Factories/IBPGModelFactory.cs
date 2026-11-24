using ZarIran.Plugin.Payments.Digipay.Models.DigiPays;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.Digipay.Factories
{
    public interface IBPGModelFactory
    {
        Task<CreateTicketModel> PrepareCreateTicketModelAsync(Order order, int paymentId);

        DeliverRequestModel PrepareDeliverRequest(int orderId, string trackingCode, List<string> productNames);

        VerifyRequestModel PrepareVerifyRequest(int paymentId, string trackingCode);

        RefundRequestModel PrepareRefundRequest(Payment.Core.Domain.Payment payment, string trackingCode);
    }
}
