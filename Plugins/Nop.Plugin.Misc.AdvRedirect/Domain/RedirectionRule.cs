using Nop.Core;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.AdvRedirect.Domain
{
    public class RedirectionRule: BaseEntity
    {
        [Required]
        public string Pattern { get; set; }

        public bool UseQueryString { get; set; }

        [Url]
        [Required]
        public string RedirectUrl { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int StoreId { get; set; }
    }


}
