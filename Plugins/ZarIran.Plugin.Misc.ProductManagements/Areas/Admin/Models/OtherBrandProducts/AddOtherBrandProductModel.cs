using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel
{
    public partial record AddOtherBrandProductModel
    {
        #region Ctor

        public AddOtherBrandProductModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int ProductId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}