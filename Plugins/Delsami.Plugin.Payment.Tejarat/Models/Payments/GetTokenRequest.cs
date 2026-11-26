using Delsami.Plugin.Payment.Tejarat.Enums;

namespace Delsami.Plugin.Payment.Tejarat.Models.Payments
{
    public record GetTokenRequest
    {
        public string terminalId { get; set; }

        public string AcceptorId { get; set; }

        public string RevertURL { get; set; }

        public string PaymentId { get; set; }

        public string RequestId { get; set; }

        public string RsaPublicKey { get; set; }

        public TransactionType transactionType { get; set; }

        public long RequestTimestamp { get; set; }

        public long amount { get; set; }

    }
}
