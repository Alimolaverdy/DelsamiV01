namespace Delsami.Plugin.Payments.Digipay.Models.DigiPays
{
    public class BasketDetailItemModel
    {
        public string sellerId { get; set; }

        public string supplierId { get; set; }

        public string productCode { get; set; }

        public string brand { get; set; }

        public int productType { get; set; }

        public int count { get; set; }

        //public string categoryId { get; set; }

    }
}
