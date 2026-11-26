using Delsami.Plugin.Misc.ProductManagement.Enums;
using Nop.Core.Configuration;

namespace Delsami.Plugin.Misc.ProductManagement
{
    public class ProductManagementSettings : ISettings
    {
        public GroupedProductOrderBy GroupedProductOrderBy { get; set; }

        public SortType SortType { get; set; }

        public int BundleProductCategoryId { get; set; }
    }
}
