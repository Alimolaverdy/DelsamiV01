namespace Delsami.Plugin.Shipping.IRPost.Models;

public class GetBarCodeRequest
{
    public int contractcode { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string postnodecode { get; set; }
    public int typecode { get; set; }
    public int servicetype { get; set; }
    public int parceltype { get; set; }
    public int sourcecode { get; set; }
    public int destcode { get; set; }
    public string sendername { get; set; }
    public string receivername { get; set; }
    public string receiverpostalcode { get; set; }
    public string senderpostalcode { get; set; }
    public int weight { get; set; }
    public string relationalkey { get; set; }
    public string senderid { get; set; }
    public string receiverid { get; set; }
    public string sendermobile { get; set; }
    public string receivermobile { get; set; }
    public string senderaddress { get; set; }
    public string receiveraddress { get; set; }
    public int insurancetype { get; set; }
    public long insuranceamount { get; set; }
    public int spsreceivertimetype { get; set; }
    public int tlsservicetype { get; set; }
    public bool tworeceiptant { get; set; }
    public bool electroreceiptant { get; set; }
    public bool iscot { get; set; }
    public bool smsservice { get; set; }
    public bool isnonstandard { get; set; }
    public int sendplacetype { get; set; }
    public long Contractorportion { get; set; }
    public string Contetnts { get; set; }
    public string Vid { get; set; }
    public int Boxsize { get; set; }
    public string Podcode { get; set; }
    public int PriceInquiry { get; set; }

}




