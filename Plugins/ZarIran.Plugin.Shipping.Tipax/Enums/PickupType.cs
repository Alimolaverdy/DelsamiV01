using System.ComponentModel;

namespace ZarIran.Plugin.Shipping.Tipax.Enums
{
    public enum PickupType
    {
        [Description("در محل مشتری")]
        CustomerSite = 10,

        [Description("در محل نمایندگی")]
        AgentSite = 20
    }
}
