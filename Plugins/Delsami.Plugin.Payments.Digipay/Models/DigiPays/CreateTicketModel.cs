namespace Delsami.Plugin.Payments.Digipay.Models.DigiPays
{
    public class CreateTicketModel
    {
        public long amount { get; set; }

        public string cellNumber { get; set; }

        public string providerId { get; set; }

        public string callbackUrl { get; set; }

        public BasketDetailModel basketDetailsDto { get; set; }


    }
}
