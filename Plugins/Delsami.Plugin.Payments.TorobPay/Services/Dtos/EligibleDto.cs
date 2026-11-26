namespace Delsami.Plugin.Payments.TorobPay.Services.Dtos
{
    public class EligibleDto
    {
        public bool successful { get; set; }
        public EligibleResponse response { get; set; }
    }

    public class EligibleResponse
    {
        public bool eligible { get; set; }
        public string title_message { get; set; }
        public string description { get; set; }
    }
}
