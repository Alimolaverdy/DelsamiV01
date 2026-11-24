using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.ProductManagements.Enums;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.Configurations
{
    public record ConfigurationModel : BaseNopModel
    {

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedVendor.Configuration.BestSellerCount")]
        public int BestSellerCount { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedVendor.Configuration.BundleProductCategoryId")]
        public int BundleProductCategoryId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedVendor.Configuration.GroupedProductOrderBy")]
        public GroupedProductOrderBy GroupedProductOrderBy { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedVendor.Configuration.SortType")]
        public SortType SortType { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ProductManagements.Configuration.BackInStockSubscriptionExpireInDays")]
        public int BackInStockSubscriptionExpireInDays { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ProductManagements.Configuration.BackInStockSubscriptionActivePeriodInDays")]
        public int BackInStockSubscriptionActivePeriodInDays { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ProductManagements.Configuration.MaxBackInStockSubsPerCustomer")]
        public int MaxBackInStockSubsPerCustomer { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ProductManagements.Configuration.MaxBackInStockSubsPerCustomerAndProduct")]
        public int MaxBackInStockSubsPerCustomerAndProduct { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedVendor.Configuration.SelectedSpecOptionId")]
        public int SelectedSpecOptionId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; } = new();
    }
}