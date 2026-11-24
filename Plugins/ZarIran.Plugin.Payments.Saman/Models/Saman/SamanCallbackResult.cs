using ZarIran.Plugin.Payment.Core.Models;

namespace ZarIran.Plugin.Payments.Saman.Models.Saman;

public class SamanCallbackResult:BasePaymentResponseModel
{
    public string MID { get; set; }

    public string TerminalId { get; set; }

    public string RefNum { get; set; }

    public string ResNum { get; set; }

    public string State { get; set; }

    public string TraceNo { get; set; }

    public string Amount { get; set; }

    public string Wage { get; set; }

    public string Rrn { get; set; }

    public string SecurePan { get; set; }

    public string Token { get; set; }

    public string HashedCardNumber { get; set; }

    public string Status { get; set; }

    public override string ToString()
    {
        return $"MID: {MID} - TerminalId: {TerminalId} - RefNum: {RefNum} - ResNum: {ResNum} - State: {State} - TraceNo: {TraceNo} - Amount: {Amount}" +
               $" - Wage: {Wage} - Rrn: {Rrn} - SecurePan: {SecurePan} - Token: {Token} - HashedCardNumber: {HashedCardNumber} - Status: {Status}";
    }
}
