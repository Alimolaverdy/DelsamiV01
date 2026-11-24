using Nop.Core;
using System;

namespace Nop.Plugin.Misc.AdvRedirect.Domain
{
    public class AdvRedirectHistory : BaseEntity
    {
        public AdvRedirectHistory(string sourceUrl, string redirectUrl)
        {
            SourceUrl = sourceUrl;
            RedirectUrl = redirectUrl;
            CreatedOnUtc = DateTime.UtcNow;
        }

        public string SourceUrl { get; set; }
        public string RedirectUrl { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
