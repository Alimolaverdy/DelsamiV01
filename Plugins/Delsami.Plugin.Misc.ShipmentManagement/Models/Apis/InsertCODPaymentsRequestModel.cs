using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Models.Apis
{
    public class InsertCODPaymentsRequestModel
    {
        public bool HasDeliveryCode { get; set; }

        public int? DevliveryCode { get; set; }

        public string ShipmentCode { get; set; }

        public List<PaymentRequestModel> Payments { get; set; }

    }
}
