using ZarIran.Plugin.Misc.CustomPdfGenerator.Enums;
using Nop.Core.Configuration;
using ZXing;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator
{
    public class CustomPdfGeneratorSettings : ISettings
    {
        public int ZarIranVendorId { get; set; }

        public BarcodeFormat BarcodeFormat { get; set; }

        public int LogoPictureId { get; set; }

        public string PhoneNumber { get; set; }

        public RotateDegree RotateDegree { get; set; }
    }
}
