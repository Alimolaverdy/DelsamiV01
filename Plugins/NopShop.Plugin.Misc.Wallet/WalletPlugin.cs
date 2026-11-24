using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using NopShop.Plugin.Misc.Wallet.Domain;

namespace NopShop.Plugin.Misc.Wallet
{
    /// <summary>
    /// Represents the Wallet Plugin
    /// </summary>
    public class WalletPlugin : BasePlugin, IMiscPlugin 
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IProductService _productService;
        private readonly WalletSettings _walletSettings;
        private readonly IPaymentPluginManager _paymentPluginManager;

        #endregion

        #region Ctor

        public WalletPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            IProductService productService,
            WalletSettings walletSettings,
            IPaymentPluginManager paymentPluginManager)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _productService = productService;
            _walletSettings = walletSettings;
            _paymentPluginManager = paymentPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Wallet/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            var product = new Nop.Core.Domain.Catalog.Product
            {
                AllowedQuantities = "1",
                CustomerEntersPrice = true,
                Name = "شارژ کیف پول",
                Published = true,
                OrderMaximumQuantity = 1,
                ProductTypeId = 5,
                MaximumCustomerEnteredPrice = 10000000,
                MinimumCustomerEnteredPrice = 10000,
            };
            await _productService.InsertProductAsync(product);

            //settings
            _walletSettings.WalletProductId = product.Id;

            var paymentMethods = await (await _paymentPluginManager
              .LoadActivePluginsAsync())
              .Where(pm => pm.PaymentMethodType == PaymentMethodType.Redirection)
              .ToListAsync();

            var paymentmethod = paymentMethods.FirstOrDefault();
            if (paymentmethod != null)
                _walletSettings.PaymentSystemName = paymentmethod.PluginDescriptor.SystemName;

            await _settingService.SaveSettingAsync(_walletSettings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Account.RewardPoints"] = "کیف پول",
                ["rewardpoints.currentbalance"] = "موجودی کیف پول شما {0} می باشد",
                ["NopShop.Plugin.Misc.Wallet.Charge.Title"] = "افزایش موجودی کیف پول",
                ["NopShop.Plugin.Misc.Wallet.Charge.Save"] = "پرداخت",
                ["NopShop.Plugin.Misc.Wallet.Charge.Amount"] = "مبلغ",
                ["NopShop.Plugin.Misc.Wallet.Fields.WalletProductId"] = "شناسه محصول",
                ["NopShop.Plugin.Misc.Wallet.Fields.WalletProductId.Hint"] = "شناسه محصول کیف پول",
                ["NopShop.Plugin.Misc.Wallet.Fields.Amount"] = "مبلغ پیش فرض",
                ["NopShop.Plugin.Misc.Wallet.Fields.Amount.Hint"] = "مبلغ پیش فرضی که کاربر جهت افزایش موجودی مشاهده می کند",
                ["NopShop.Plugin.Misc.Wallet.Fields.MaxAmount"] = "حداکثر مبلغ",
                ["NopShop.Plugin.Misc.Wallet.Fields.MaxAmount.Hint"] = "حداکثر مبلغ قابل افزایش کیف پول",
                ["NopShop.Plugin.Misc.Wallet.Fields.MinWallet"] = "حداقل مبلغ",
                ["NopShop.Plugin.Misc.Wallet.Fields.MinWallet.Hint"] = "حداقل مبلغ قابل افزایش کیف پول",
                ["NopShop.Plugin.Misc.Wallet.Fields.PaymentSystemName"] = "روش پرداخت",
                ["NopShop.Plugin.Misc.Wallet.Fields.PaymentSystemName.Hint"] = "نام سیستمی روش پرداخت",
                ["RewardPoints.Fields.Points"] = "مبلغ",
                ["RewardPoints.Fields.PointsBalance"] = "موجودی",
                ["checkout.userewardpoints"] = "استفاده از کیف پول، موجودی کیف پول {0}",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {             
            await _settingService.DeleteSettingAsync<WalletSettings>();
             
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("NopShop.Plugin.Misc.Wallet");

            await base.UninstallAsync();
        }

        #endregion
    }
}
