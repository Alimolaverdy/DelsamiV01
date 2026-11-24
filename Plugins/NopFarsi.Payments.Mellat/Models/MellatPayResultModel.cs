using ZarIran.Plugin.Payment.Core.Models;

namespace NopFarsiIR.Payments.Mellat.Models;

public class MellatPayResultModel : BasePaymentResponseModel
{
    public string resCode { get; set; }
    public string refId { get; set; }
    public string saleOrderId { get; set; }
    public string saleReferenceId { get; set; }
    public string cardHolderPan { get; set; }
    public string creditCardSaleResponseDetail { get; set; }
    public string finalAmount { get; set; }

    public override string ToString()
    {
        return $"resCode:{resCode} - refId:{refId} - saleOrderId:{saleOrderId} - saleReferenceId:{saleReferenceId} - cardHolderPan:{cardHolderPan} -" +
               $" creditCardSaleResponseDetail:{creditCardSaleResponseDetail} - finalAmount:{finalAmount}";
    }
}
