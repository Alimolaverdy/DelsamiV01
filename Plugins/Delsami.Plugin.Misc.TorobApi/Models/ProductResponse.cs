using Newtonsoft.Json;
using Delsami.Plugin.Misc.TorobApi.Infrastructure;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.TorobApi.Models
{
    public class ProductResponse
    {
        public int Count { get; set; }
        public int Max_Pages { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
