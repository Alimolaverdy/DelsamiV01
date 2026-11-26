using Nop.Services.Common.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Delsami.Plugin.Misc.CustomPdfGenerator.Model
{
    public class CustomOrderSource
    {
        #region Ctor

        public CustomOrderSource()
        {
            VendorsProducts = new();
            DelsamiProducts = new();
            VendorsTotals = new();
            DelsamiTotals = new();
            Totals = new();
            FooterTextColumn1 = new();
            FooterTextColumn2 = new();
            IsProformaInvoice = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the logo binary
        /// </summary>
        public byte[] LogoData { get; set; }

        public bool IsProformaInvoice { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        [DisplayName("Pdf.OrderDate")]
        public DateTime OrderDateUser { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        [DisplayName("Pdf.Order")]
        public string OrderNumberText { get; set; }

        /// <summary>
        /// Gets or sets store location
        /// </summary>
        public string StoreUrl { get; set; }

        /// <summary>
        /// Gets or sets a collection of order items
        /// </summary>
        public List<CustomProductItem> VendorsProducts { get; set; }

        public List<CustomProductItem> DelsamiProducts { get; set; }

        /// <summary>
        /// Gets or sets the billing address
        /// </summary>
        [DisplayName("Pdf.BillingInformation")]
        public AddressItem BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address
        /// </summary>
        [DisplayName("Pdf.ShippingInformation")]
        public AddressItem ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display product SKU in the invoice document
        /// </summary>
        public bool ShowSkuInProductList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display vendor name in the invoice document
        /// </summary>
        public bool ShowVendorInProductList { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute description
        /// </summary>
        public string CheckoutAttributes { get; set; }

        /// <summary>
        /// Gets or sets order totals
        /// </summary>
        public CustomInvoiceTotals DelsamiTotals { get; set; }

        public CustomInvoiceTotals VendorsTotals { get; set; }

        public int VendorsProductsQuantity { get; set; }

        public int DelsamiProductsQuantity { get; set; }

        public InvoiceTotals Totals { get; set; }
        /// <summary>
        /// Gets or sets order notes
        /// </summary>
        [DisplayName("Pdf.OrderNotes")]
        public List<(string, string)> OrderNotes { get; set; }

        /// <summary>
        /// Gets or sets the text that will appear at the bottom of invoice (column 1)
        /// </summary>
        public List<string> FooterTextColumn1 { get; set; }

        /// <summary>
        /// Gets or sets the text that will appear at the bottom of invoice (column 2)
        /// </summary>
        public List<string> FooterTextColumn2 { get; set; }

        public string TotalQuantity { get; set; }

        public string OrderDiscount { get; set; }

        public string CustomerNationalCode { get; set; }

        public string CustomerZipPostalCode { get; set; }

        public string ShippingPrice { get; set; }

        public string InvoiceId { get; set; }

        public string CompanyNameTitle { get; set; }

        public string CompanyNationalCodeTitle { get; set; }

        public string CompanyRegisterNumberTitle { get; set; }

        public string CompanyAddressTitle { get; set; }

        public string CompanyPostalCodeTitle { get; set; }

        public string CompanyPhoneNumberTitle { get; set; }

        public string CompanyName { get; set; }

        public string CompanyNationalCode { get; set; }

        public string CompanyRegisterNumber { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyPostalCode { get; set; }

        public string CompanyPhoneNumber { get; set; }

        #endregion
    }
}
