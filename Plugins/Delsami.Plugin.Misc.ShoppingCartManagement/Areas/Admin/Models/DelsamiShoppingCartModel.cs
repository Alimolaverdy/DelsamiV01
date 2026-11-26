using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Models
{
    public record DelsamiShoppingCartModel : ShoppingCartModel
    {

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShoppingCartManagement.Username")]
        public string CustomerUserName { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShoppingCartManagement.CreatedOnUtc")]
        public string CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShoppingCartManagement.UpdatedOnUtc")]
        public string UpdatedOnUtc { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShoppingCartManagement.ReferralCode")]
        public string ReferralCode { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShoppingCartManagement.TotalShoppingCartPrice")]
        public string TotalShoppingCartPrice{ get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShoppingCartManagement.CustomerRoleNames")]
        public List<string> CustomerRoleNames { get; set; }
    }
}
