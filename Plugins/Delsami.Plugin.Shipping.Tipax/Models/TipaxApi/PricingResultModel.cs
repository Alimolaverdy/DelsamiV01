using ZarIran.Plugin.Shipping.Tipax.Enums;

namespace ZarIran.Plugin.Shipping.Tipax.Models.TipaxApi
{
    public class PricingResultModel
    {
        public int packNum { get; set; }

        public string originCity { get; set; }

        public string destiationCity { get; set; }

        public RegularRate regularRate { get; set; }

        public class RegularRate
        {
            public ServiceType serviceId { get; set; }

            public string serviceTitle { get; set; }

            public decimal transportationCost { get; set; }

            public int compensation { get; set; }

            public int noticeOfDelivery { get; set; }

            public int tracking { get; set; }

            public Packing packing { get; set; }

            public decimal beforeTaxAndDiscount { get; set; }

            public decimal tax { get; set; }

            public decimal finalPrice { get; set; }

            public decimal discount { get; set; }
        }
    }
}
