using System.Collections.Generic;
using System.ComponentModel;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Model
{
    public class CustomInvoiceTotals
    {
        #region Ctor

        public CustomInvoiceTotals()
        {
            GiftCards = new();
            TaxRates = new();
        }

        #endregion

        #region Properties

        [DisplayName("Pdf.SubTotal")]
        public string SubTotal { get; set; }

        [DisplayName("Pdf.Discount")]
        public string Discount { get; set; }

        public List<string> GiftCards { get; set; }

        [DisplayName("Pdf.Shipping")]
        public string Shipping { get; set; }

        [DisplayName("Pdf.PaymentMethodAdditionalFee")]
        public string PaymentMethodAdditionalFee { get; set; }

        [DisplayName("Pdf.Tax")]
        public string Tax { get; set; }

        public List<string> TaxRates { get; set; }

        public string RewardPoints { get; set; }

        public string OrderTotalWithoutShipping { get; set; }

        public string OrderTotal { get; set; }

        #endregion
    }
}
