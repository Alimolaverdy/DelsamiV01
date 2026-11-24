using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.TorobPay
{
    public class TorobPaySettings : ISettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
        public int TokenCacheMinutes { get; set; }
        public decimal MinimumAllowedAmount { get; set; }
        public decimal MaximumAllowedAmount { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
