using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.VirtualProducts
{
    public class VirtualProductPreviewModel
    {
        public IList<VirtualProductPreviewItemModel> Items { get; set; } = new List<VirtualProductPreviewItemModel>();

        public int MainProductId { get; set; }

    }
    public class VirtualProductPreviewItemModel
    {
        public bool Selected { get; set; }

        public bool CanSelect { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
