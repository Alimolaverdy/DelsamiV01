using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Models
{
    public record ZarIranShoppingCartModel : ShoppingCartModel
    {

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShoppingCartManagement.Username")]
        public string CustomerUserName { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShoppingCartManagement.CreatedOnUtc")]
        public string CreatedOnUtc { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShoppingCartManagement.UpdatedOnUtc")]
        public string UpdatedOnUtc { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShoppingCartManagement.ReferralCode")]
        public string ReferralCode { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShoppingCartManagement.TotalShoppingCartPrice")]
        public string TotalShoppingCartPrice{ get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ShoppingCartManagement.CustomerRoleNames")]
        public List<string> CustomerRoleNames { get; set; }
    }
}
