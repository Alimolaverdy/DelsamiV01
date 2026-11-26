using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.BundleProducts
{
    public record BundleProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductId { get; set; }

        #endregion
    }
}