using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel
{
    public record OtherBrandProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductId { get; set; }

        #endregion
    }
}