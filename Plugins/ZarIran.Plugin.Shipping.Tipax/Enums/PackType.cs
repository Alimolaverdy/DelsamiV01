using System.ComponentModel;

namespace ZarIran.Plugin.Shipping.Tipax.Enums
{
    public enum PackType
    {
        [Description("پاکت")]
        Envelope = 10,

        [Description("بسته")]
        Carton = 20,

        [Description("مینی پک")]
        MiniPack = 50,
    }
}
