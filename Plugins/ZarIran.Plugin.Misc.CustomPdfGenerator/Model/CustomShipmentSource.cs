using Nop.Core.Domain.Common;
using Nop.Services.Common.Pdf;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Model
{
    public class CustomShipmentSource
    {
        #region Ctor

        public CustomShipmentSource()
        {
            Products = new();
        }

        #endregion

        #region Properties

        public string CityName { get; set; }

        public string StateProvinceName { get; set; }

        [DisplayName("Pdf.ShippingInformation")]
        public AddressItem Address { get; set; }
        [DisplayName("Pdf.Order")]
        public string OrderNumberText { get; set; }

        [DisplayName("Pdf.Shipment")]
        public string ShipmentNumberText { get; set; }

        [DisplayName("Pdf.Bracode")]
        public MemoryStream BracodeStream { get; set; }

        public string BracodeContent { get; set; }

        [DisplayName("Pdf.Logo")]
        public MemoryStream LogoStream { get; set; }

        public string ReciverCustomerFullName { get; set; }
        public string OrderCustomerFullName { get; set; }

        public bool IsDifferentCustomer { get; set; }

        public string DeliveryTime { get; set; }

        public string PostalCode { get; set; }

        [DisplayName("Pdf.ZarIranPhoneNumber")]
        public string ZarIranPhoneNumber { get; set;
        }
        public List<ProductItem> Products { get; set; }



        #endregion
    }
}
