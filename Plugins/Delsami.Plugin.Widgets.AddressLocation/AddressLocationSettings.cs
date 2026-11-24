using ZarIran.Plugin.Widgets.AddressLocation.Enums;
using Nop.Core.Configuration;

namespace ZarIran.Plugin.Widgets.AddressLocation
{
    public class AddressLocationSettings : ISettings
    {
        public int LatitudeAddressAttributeId { get; set; }

        public int LongitudeAddressAttributeId { get; set; }

        public int DigiKalaAddressAttributeId { get; set; }

        public string NeshanServiceApiKey { get; set; }
        
        public string NeshanMapApiKey { get; set; }

        public int NeshanServiceTimeoutAsMilliseconds { get; set; }

        public MapType MapType { get; set; }
    }
}
