using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts
{
    public record BundleProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductId { get; set; }

        #endregion
    }
}