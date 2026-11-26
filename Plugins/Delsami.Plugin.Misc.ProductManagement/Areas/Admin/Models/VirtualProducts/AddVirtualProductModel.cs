using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.VirtualProducts
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