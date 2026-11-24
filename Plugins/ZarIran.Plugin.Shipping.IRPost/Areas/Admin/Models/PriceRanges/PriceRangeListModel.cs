using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges
{
    public record PriceRangeListModel : BasePagedListModel<PriceRangeModel>
    {
    }
}
