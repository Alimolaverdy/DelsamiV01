using System.ComponentModel;

namespace ZarIran.Plugin.Shipping.Tipax.Enums
{
    public enum PackageContent
    {
        [Description("مایعات-شکستنی")]
        FragileLiquids = 1,

        [Description("عمومی")]
        General1 = 3,

        [Description("اوراق و اسناد بانکی(پاکت)")]
        BankDocumentsEnvelope = 7,

        [Description("عمومی")]
        General2 = 9,

        [Description("عمومی")]
        General3 = 10
    }
}
