using Nop.Core.Configuration;
using System;

namespace ZarIran.Plugin.Shipping.Core
{
    public class ShippingCoreSettings : ISettings
    {
        public TimeSpan LogisticStartTime { get; set; }

        public TimeSpan LogisticEndTime { get; set; }
    }
}
