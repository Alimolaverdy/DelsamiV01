using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Models.CustomerShebaNumbers
{
    public record CustomerShebaNumberModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Number")]
        public string Number { get; set; }
    }
}
