using ZarIran.Plugin.Misc.ProductManagements.Enums;
using Nop.Core.Configuration;

namespace ZarIran.Plugin.Misc.ProductManagements
{
    public class ProductManagementSettings : ISettings
    {
        public bool Initialized { get; set; }

        public int BackInStockSubscriptionNotifiedPeriodInDays { get; set; }
        public int BackInStockSubscriptionActivePeriodInDays { get; set; }
        public int BackInStockSubscriptionExpireInDays { get; set; }
        public int MaxBackInStockSubsPerCustomer { get; set; }
        public int MaxBackInStockSubsPerCustomerAndProduct { get; set; }
        public GroupedProductOrderBy GroupedProductOrderBy { get; set; }
        public SortType SortType { get; set; }

        public int BundleProductCategoryId { get; set; }

        public int BestSellerCount { get; set; }

    }
}
