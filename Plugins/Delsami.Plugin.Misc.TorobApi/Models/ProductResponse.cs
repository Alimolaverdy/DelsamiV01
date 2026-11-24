using Newtonsoft.Json;
using ZarIran.Plugin.Misc.TorobApi.Infrastructure;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.TorobApi.Models
{
    public class ProductResponse
    {
        public int Count { get; set; }
        public int Max_Pages { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
