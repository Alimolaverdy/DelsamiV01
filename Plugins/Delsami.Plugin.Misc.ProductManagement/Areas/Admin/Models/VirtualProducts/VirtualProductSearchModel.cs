using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.VirtualProducts
{
    public record VirtualProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int ProductId { get; set; }

        #endregion
    }
}