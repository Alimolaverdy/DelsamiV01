using Nop.Core.Configuration;

namespace Delsami.Plugin.Misc.ShipmentManagement
{
    public class ShipmentManagementSettings : ISettings
    {
        public bool DeliveryCodeToCustomerEnabled { get; set; }

        public string DeliveryCodeToCustomerMessageTemplate { get; set; }

        public string ShipmentIdIndicatorWordInFile { get; set; }

        public string TrackingNumberIndicatorWordInFile { get; set; }

        public string IRPostTrackingLink { get; set; }
        public string TipaxTrackingLink { get; set; }



    }
}
