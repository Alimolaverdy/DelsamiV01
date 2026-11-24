using ZarIran.Plugin.Shipping.Tipax.Enums;

namespace ZarIran.Plugin.Shipping.Tipax.Models.TipaxApi
{
    public class PricingItemRequestModel
    {
        public Route origin { get; set; }

        public Route destination { get; set; }

        public decimal weight { get; set; }

        public decimal length { get; set; }

        public decimal width { get; set; }

        public decimal height { get; set; }

        public Packing packingId { get; set; }

        public PackageContent packageContentId { get; set; }

        public PackType packType { get; set; }

        public PaymentType paymentType { get; set; }

        public PickupType pickupType { get; set; }

        public DistributionType distributionType { get; set; }

        public ServiceType serviceId { get; set; }

        public class Route
        {
            public int cityId { get; set; }
        }
    }
}
