using System.Collections.Generic;

namespace Delsami.Plugin.Payments.Digipay.Models.DigiPays
{
    public class BasketDetailModel
    {
        public string basketId { get; set; }

        public List<BasketDetailItemModel> items { get; set; }
    }
}
