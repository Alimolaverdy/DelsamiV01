using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.Core.Model
{
    public class PackingResultModel
    {
        public CartonWithVolumeModel Carton { get; set; }

        public List<ProductWithVolumeModel> Products { get; set; } = new();
    }
}
