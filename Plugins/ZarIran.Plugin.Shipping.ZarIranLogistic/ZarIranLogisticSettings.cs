using Nop.Core.Configuration;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic
{
    public class ZarIranLogisticSettings : ISettings
    {

        public int ShippingMethodId { get; set; }

        public int PickupTruckShippingMethodId { get; set; }

        public int ScheduledShippingMethodId { get; set; }

        public int ScheduledShippingMethodWithTruckId { get; set; }

        public string ApiBaseUrl { get; set; }

        public string ApiToken { get; set; }

        public decimal Price { get; set; }

        public decimal PickupTruckPrice { get; set; }

        public decimal ScheduledPrice { get; set; }

        public decimal ScheduledPriceForHeavyOrBigBasket { get; set; }

        public decimal WeightMustCarryByPickupTruckInGrams { get; set; }
        
        public decimal VolumeMustCarryByPickupTruckInCM3 { get; set; }

        public decimal OverSizeProductVolumeMustCarryByPickupTruckInCM3 { get; set; }

    }
}
