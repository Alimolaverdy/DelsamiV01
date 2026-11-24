using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.Core.Events
{
    public class BackInStockSubscriptionTriggeredEvent
    {
        public string FullName { get; set; }

        public string ProductUrl { get; set; }

        public string ProductName { get; set; }

        public string MobilePhoneNumber { get; set; }
    }
}
