using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopShop.Plugin.Misc.Wallet.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }
         
        public bool HideGeneralBlock { get; set; }

        [NopResourceDisplayName("NopShop.Plugin.Misc.Wallet.Fields.WalletProductId")]
        public int WalletProductId { get; set; }

        [NopResourceDisplayName("NopShop.Plugin.Misc.Wallet.Fields.Amount")]
        public int Amount { get; set; }

        [NopResourceDisplayName("NopShop.Plugin.Misc.Wallet.Fields.MaxAmount")]
        public int MaxAmount { get; set; }

        [NopResourceDisplayName("NopShop.Plugin.Misc.Wallet.Fields.MinWallet")]
        public int MinWallet { get; set; }

        [NopResourceDisplayName("NopShop.Plugin.Misc.Wallet.Fields.PaymentSystemName")]
        public string PaymentSystemName { get; set; }

        #endregion
    }
}