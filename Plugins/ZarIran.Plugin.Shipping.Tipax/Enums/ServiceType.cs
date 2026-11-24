using System.ComponentModel;

namespace ZarIran.Plugin.Shipping.Tipax.Enums
{
    public enum ServiceType
    {
        [Description("ارسال زمینی همان روز")]
        SameDayGround = 1,

        [Description("ارسال زمینی یک روزه")]
        OneDayGround = 2,

        [Description("ارسال زمینی دو روزه")]
        TwoDayGround = 3,

        [Description("اکسپرس ویژه بین شهری")]
        InterCityExpress = 5,

        [Description("بین الملل")]
        International = 6,

        [Description("اکسپرس درون شهری")]
        IntraCityExpress = 7
    }
}
