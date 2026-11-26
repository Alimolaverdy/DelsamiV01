using Delsami.Plugin.Payment.Core.Models;

namespace Delsami.Plugin.Payment.Tejarat.Models.Payments;

public class TejaratPayResultModel : BasePaymentResponseModel
{
    public string acceptorId { get; set; }
    public string Amount { get; set; }
    public string retrievalReferenceNumber { get; set; }
    public string token { get; set; }
    public string systemTraceAuditNumber { get; set; }
    public string responseCode { get; set; }
    public string maskedPan { get; set; }

    public override string ToString()
    {
        return $"acceptorId: {acceptorId} - Amount={Amount} - retrievalReferenceNumber: {retrievalReferenceNumber} - token: {token} -" +
               $" systemTraceAuditNumber: {systemTraceAuditNumber} - responseCode: {responseCode} - maskedPan: {maskedPan}";
    }
}
