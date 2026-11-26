using System.Collections.Generic;

namespace Delsami.Plugin.Misc.TorobApi.Models
{
    public class ProductRequest
    {
        //public string Token { get; set; }
        //public bool? Variation { get; set; }
        //public int? Limit { get; set; }
        //public int? Page { get; set; }
        //public List<int> ProductIds { get; set; }
        //public List<string> Slugs { get; set; }
        //public bool AutoUpdate { get; set; } = true;

        public string Page_Unique { get; set; }

        public string Page_Url { get; set; }

        public int? Page { get; set; }
    }
}
