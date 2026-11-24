using System;
using Nop.Core.Configuration;

namespace NopFarsiIR.Plugin.Payments.BankPasargad
{
    public class BankPasargadPaymentSettings : ISettings
    {
        public string Password { get; set; }

        public string Username { get; set; }

        public string TerminalCode { get; set; }

        public bool IsToman { get; set; }

        public int CurrencyId { get; set; }

        public bool ConfirmTransaction { get; set; }
    }
}
