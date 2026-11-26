using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models
{
    public record CustomerAttributeValidationModel : BaseNopEntityModel
    {
        public CustomerAttributeValidationModel()
        {
            AvailableCustomerAttributes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId")]
        public int CustomerAttributeId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId")]
        public string CustomerAttributeName { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern")]
        public string RegexPattern { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.IsNationalCode")]
        public bool IsNationalCode { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.PreventFromShoppingInInvalidState")]
        public bool PreventFromShoppingInInvalidState { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.Active")]
        public bool Active { get; set; }

        public List<SelectListItem> AvailableCustomerAttributes { get; set; }
    }
}
