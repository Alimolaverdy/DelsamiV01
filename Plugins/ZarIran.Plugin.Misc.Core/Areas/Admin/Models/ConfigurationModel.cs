using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.Core.Areas.Admin.Models
{
    public class ConfigurationModel

    {
        public ConfigurationModel()
        {
            AvailableCustomerRoleIds = new();
        }


        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.ManualGoldPrice")]
        public decimal ManualGoldPrice { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.UseManualGoldPrice")]
        public bool UseManualGoldPrice { get; set; }

		[NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.GoldPriceAdjustment")]
		public decimal GoldPriceAdjustment { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.AllowGoldPriceAdjustment")]
        public bool AllowGoldPriceAdjustment { get; set; }

		[NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.NationalCodeAttributeId")]
        public int NationalCodeAttributeId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.AllProductsCategoryId")]
        public int AllProductsCategoryId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.Tax")]
        public decimal Tax { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.TradeDiscountId")]
        public int TradeDiscountId { get; set; }


        [NopResourceDisplayName("ZarIran.Plugin.Misc.Core.Configuration.CoWorkerRoleId")]
        public int CoWorkerRoleId { get; set; }

        public List<SelectListItem> AvailableCustomerRoleIds { get; set; }

    }
}
