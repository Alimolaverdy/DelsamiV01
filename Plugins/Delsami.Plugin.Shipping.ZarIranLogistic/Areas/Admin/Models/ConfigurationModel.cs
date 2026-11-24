using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Areas.Admin.Models
{
    public class ConfigurationModel
    {
        public ConfigurationModel()
        {
            AvailableShippingMethods = new List<SelectListItem>();
        }

        [NopResourceDisplayName("ZarIran.Plugins.Shipping.ZarIranLogistic.ShippingMethodId")]
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugins.Shipping.ZarIranLogistic.PickupTrucShippingMethodId")]
        public int PickupTruckShippingMethodId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugins.Shipping.ZarIranLogistic.ScheduledShippingMethodId")]
        public int ScheduledShippingMethodId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugins.Shipping.ZarIranLogistic.ScheduledShippingMethodWithTruckId")]
        public int ScheduledShippingMethodWithTruckId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.PickupTruckPrice")]
        public decimal PickupTruckPrice { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.ScheduledPrice")]
        public decimal ScheduledPrice { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.ScheduledPriceForHeavyOrBigBasket")]
        public decimal ScheduledPriceForHeavyOrBigBasket { get; set; }

        [NopResourceDisplayName("ZarIran.Plugins.Shipping.ZarIranLogistic.ApiBaseUrl")]
        public string ApiBaseUrl { get; set; }

        [NopResourceDisplayName("ZarIran.Plugins.Shipping.ZarIranLogistic.ApiToken")]
        public string ApiToken { get; set; }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.WeightMustCarryByPickupTruckInGrams")]
        public decimal WeightMustCarryByPickupTruckInGrams { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.volumeMustCarryByPickupTruckInCM3")]
        public decimal VolumeMustCarryByPickupTruckInCM3 { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Shipping.ZarIranLogistic.OverSizeProductVolumeMustCarryByPickupTruckInCM3")]
        public decimal OverSizeProductVolumeMustCarryByPickupTruckInCM3 { get; set; }


    }
}
