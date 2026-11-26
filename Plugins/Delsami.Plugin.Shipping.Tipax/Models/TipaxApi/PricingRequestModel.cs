using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.Tipax.Models.TipaxApi
{
    public class PricingRequestModel
    {
        public List<PricingItemRequestModel> packageInputs { get; set; }
    }
}
