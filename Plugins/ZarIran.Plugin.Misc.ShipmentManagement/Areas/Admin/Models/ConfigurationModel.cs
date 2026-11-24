using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.DeliveryCodeToCustomerEnabled")]
        public bool DeliveryCodeToCustomerEnabled { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.DeliveryCodeToCustomerMessageTemplate")]
        public string DeliveryCodeToCustomerMessageTemplate { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.DeliverdToCustomerEnabled")]
        public bool DeliverdToCustomerEnabled { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.DeliverdToCustomerMessageTemplate")]
        public string DeliverdToCustomerMessageTemplate { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.ShipmentIdIndicatorWordInFile")]
        public string ShipmentIdIndicatorWordInFile { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.TrackingNumberIndicatorWordInFile")]
        public string TrackingNumberIndicatorWordInFile { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.IRPostTrackingLink")]
        public string IRPostTrackingLink { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShipmentManagement.Configuration.TipaxTrackingLink")]
        public string TipaxTrackingLink { get; set; }
    }
}
