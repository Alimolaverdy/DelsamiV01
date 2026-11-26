namespace Delsami.Plugin.Misc.ShipmentManagement.Models
{
    public record DelsamiOrderItemModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceInclTax { get; set; }
    }
}
