namespace ZarIran.Plugin.Misc.ShipmentManagement.Models
{
    public record ZarIranOrderItemModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceInclTax { get; set; }
    }
}
