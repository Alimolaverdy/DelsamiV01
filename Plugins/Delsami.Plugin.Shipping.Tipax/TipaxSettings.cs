using Delsami.Plugin.Shipping.Tipax.Enums;
using Nop.Core.Configuration;

namespace Delsami.Plugin.Shipping.Tipax
{
    public class TipaxSettings : ISettings
    {
        public int ShippingMethodId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ApiKey { get; set; }

        public string BaseUrl { get; set; }

        public Packing Packing { get; set; }

        public PackageContent PackageContent { get; set; }

        public bool IsUnusual { get; set; }

        public PackType PackType { get; set; }

        public PaymentType PaymentType { get; set; }

        public PickupType PickupType { get; set; }

        public DistributionType DistributionType { get; set; }

        public ServiceType ServiceType { get; set; }

        public int TimeoutSeconds { get; set; }
        public decimal Multiplier { get; set; }
    }
}
