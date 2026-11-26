using Delsami.Plugin.Misc.CustomPdfGenerator.Enums;
using Nop.Core.Configuration;
using ZXing;

namespace Delsami.Plugin.Misc.CustomPdfGenerator
{
    public class CustomPdfGeneratorSettings : ISettings
    {
        public int DelsamiVendorId { get; set; }

        public BarcodeFormat BarcodeFormat { get; set; }

        public int LogoPictureId { get; set; }

        public string PhoneNumber { get; set; }

        public RotateDegree RotateDegree { get; set; }
    }
}
