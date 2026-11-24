using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopFarsiIR.Plugin.Payments.BankPasargad.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BankPasargad.Fields.Password")]
        public string Password { get; set; }

        public bool PasswordOverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BankPasargad.Fields.Username")]
        public string Username { get; set; }

        public bool UsernameOverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BankPasargad.Fields.TerminalCode")]
        public string TerminalCode { get; set; }

        public bool UserPasswordOverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BankPasargad.Fields.IsToman")]
        public bool IsToman { get; internal set; }

        public bool IsTomanOverrideForStore { get; set; }

        public int CurrencyId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BankPasargad.Fields.ConfirmTransaction")]
        public bool ConfirmTransaction { get; set; }

        public bool IsConfirmTransactionOverrideForStore { get; set; }


    }
}
