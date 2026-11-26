using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Misc.ProductManagement.Enums;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Configurations
{
    public record ConfigurationModel : BaseNopModel
    {

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedVendor.Configuration.GroupedProductOrderBy")]
        public GroupedProductOrderBy GroupedProductOrderBy { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedVendor.Configuration.SortType")]
        public SortType SortType { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedVendor.Configuration.BundleProductCategoryId")]
        public int BundleProductCategoryId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; } = new();
    }
}