namespace Delsami.Plugin.Misc.ShipmentManagement.Models
{
    public record DelsamiCustomerInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShippingAddress { get; set; }
    }
}
