using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.VirtualProducts
{
    public record VirtualProductModel : BaseNopEntityModel
    {
        #region Properties

        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.VirtualProduct.Fields.Product")]
        public string ProductName { get; set; }

        #endregion
    }
}
