using ZarIran.Plugin.Payment.Core.Models;

namespace ZarIran.Plugin.Payments.Saman.Models.Saman;

public class VerifyTransactionResult:BasePaymentResponseModel
{
    public VerifyTransactionResultDetail TransactionDetail { get; set; }

    public int ResultCode { get; set; }

    public string ResultDescription { get; set; }

    public bool Success { get; set; }

    public override string ToString()
    {
        return $"ResultCode: {ResultCode} - ResultDescription: {ResultDescription} - Success: {Success} --- TransactionDetails = RefNum: " +
               $"{TransactionDetail.RefNum} - RRN: {TransactionDetail.RRN} - TerminalNumber: {TransactionDetail.TerminalNumber} - " +
               $"OrginalAmount: {TransactionDetail.OrginalAmount} - MaskedPan:{TransactionDetail.MaskedPan} - HashedPan:{TransactionDetail.HashedPan} - " +
               $"StraceDate:{TransactionDetail.StraceDate} - SecurePan: {TransactionDetail.StraceNo}";
    }
}
