namespace Delsami.Plugin.Shipping.IRPost.Models;

public class GetBarCodeResponse
{
    public Data data { get; set; }
    public bool Status { get; set; }
    public string Message { get; set; }
    public string Barcode { get; set; }
    public string Barcode2 { get; set; }
    public string Credit { get; set; }
    public string Discount { get; set; }
    public string discountpostfare { get; set; }
    public string podcode { get; set; }


}


public class Data
{
    public long Totalprice { get; set; }
    public long Postprice { get; set; }
    public long Postfare { get; set; }
    public long Insuranceprice { get; set; }
    public int Tax { get; set; }
    public long Sendplaceprice { get; set; }
    public long Smsprice { get; set; }
    public long Nonstandardprice { get; set; }
}