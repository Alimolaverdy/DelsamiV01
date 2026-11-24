using Nop.Core.Configuration;

namespace ZarIran.Plugin.Misc.Core
{
    public class CoreSettings : ISettings
    {
        public decimal ManualGoldPrice { get; set; }
        public bool UseManualGoldPrice { get; set; }
        public decimal GoldPriceAdjustment { get; set; }
        public bool AllowPriceAdjustment { get; set; }
        public decimal Tax { get; set; }
        public  int NationalCodeAttributeId { get; set; }
        public  int AllProductsCategoryId { get; set; }
        public int TradeDiscountId { get; set; }
        public int CoWorkerRoleId { get; set; }
    }
}
