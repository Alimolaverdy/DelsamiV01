using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts
{
    public partial record AddBundleProductModel
    {
        #region Ctor

        public AddBundleProductModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int ProductId { get; set; }
        public int Count { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}
