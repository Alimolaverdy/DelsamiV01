using Nop.Core;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Domain
{
    public class CustomerAttributeValidation : BaseEntity
    {
        public int CustomerAttributeId { get; set; }

        public string RegexPattern { get; set; }

        public bool IsNationalCode { get; set; }

        public bool PreventFromShoppingInInvalidState { get; set; }

        public bool Active { get; set; }


    }
}
