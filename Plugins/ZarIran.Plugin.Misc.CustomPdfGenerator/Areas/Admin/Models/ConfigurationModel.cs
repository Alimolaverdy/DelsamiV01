using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.CustomPdfGenerator.Enums;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using ZXing;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableVendors = new();
            AvailableBarcodeFormats = new();
        }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.ZarIranVendorId")]
        public int ZarIranVendorId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.BarcodeFormat")]
        public BarcodeFormat BarcodeFormat { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.Pciture")]
        public int LogoPictureId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.RotateDegree")]
        public RotateDegree RotateDegree { get; set; }

        public List<SelectListItem> AvailableVendors { get; set; }
        public List<SelectListItem> AvailableBarcodeFormats { get; set; }
    }
}
