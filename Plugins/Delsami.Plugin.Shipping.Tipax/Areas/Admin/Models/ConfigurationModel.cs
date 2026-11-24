using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Shipping.Tipax.Enums;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.Tipax.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableShippingMethods = new List<SelectListItem>();
        }

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

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.Tipax.Configuration.TimeoutSeconds")]
        public int TimeoutSeconds { get; set; }
        public decimal Multiplier { get; set; }

    }
}
