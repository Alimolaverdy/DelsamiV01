using System.ComponentModel;

namespace ZarIran.Plugin.Shipping.Tipax.Enums
{
    public enum DistributionType
    {
        [Description("تحویل در محل مشتری")]
        CustomerLocation = 10,

        [Description("تحویل در نمایندگی")]
        AgencyLocation = 20
    }
}
