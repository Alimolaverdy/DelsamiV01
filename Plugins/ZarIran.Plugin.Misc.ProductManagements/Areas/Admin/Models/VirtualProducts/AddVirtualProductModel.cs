using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.VirtualProducts
{
    public record AddVirtualProductModel : BaseNopModel
    {
        #region Ctor

        public AddVirtualProductModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int MainProductId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}