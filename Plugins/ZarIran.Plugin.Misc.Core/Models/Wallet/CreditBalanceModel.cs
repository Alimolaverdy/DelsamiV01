using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.Core.Models.Wallet
{
    public record CreditBalanceModel : BaseNopModel
    {
        public string Amount { get; set; }
    }
}
