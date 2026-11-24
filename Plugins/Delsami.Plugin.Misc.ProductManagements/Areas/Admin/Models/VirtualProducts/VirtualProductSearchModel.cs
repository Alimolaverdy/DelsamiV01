using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.VirtualProducts
{
    public record VirtualProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductId { get; set; }

        #endregion
    }
}