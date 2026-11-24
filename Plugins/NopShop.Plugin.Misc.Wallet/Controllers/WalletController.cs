using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using NopShop.Plugin.Misc.Wallet.Domain;
using NopShop.Plugin.Misc.Wallet.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NopShop.Plugin.Misc.Wallet.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class WalletController : BasePluginController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly WalletSettings _walletSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IProductService _productService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public WalletController(IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext,
            ICustomerService customerService,
            RewardPointsSettings rewardPointsSettings,
            IOrderModelFactory orderModelFactory,
            WalletSettings walletSettings,
            OrderSettings orderSettings,
            IShoppingCartService shoppingCartService,
            IPaymentPluginManager paymentPluginManager,
            IProductService productService,
            ICheckoutModelFactory checkoutModelFactory,
            CaptchaSettings captchaSettings,
            IOrderProcessingService orderProcessingService,
            IPaymentService paymentService,
            IWebHelper webHelper,
            IOrderService orderService,
            IPriceFormatter priceFormatter)
        {
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _customerService = customerService;
            _rewardPointsSettings = rewardPointsSettings;
            _orderModelFactory = orderModelFactory;
            _walletSettings = walletSettings;
            _orderSettings = orderSettings;
            _shoppingCartService = shoppingCartService;
            _paymentPluginManager = paymentPluginManager;
            _productService = productService;
            _checkoutModelFactory = checkoutModelFactory;
            _captchaSettings = captchaSettings;
            _orderProcessingService = orderProcessingService;
            _paymentService = paymentService;
            _webHelper = webHelper;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
        }

        #endregion

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<WalletSettings>(storeId);

            var product = await _productService.GetProductByIdAsync(settings.WalletProductId);
            //prepare common properties
            model.ActiveStoreScopeConfiguration = storeId;
            model.Amount = settings.Amount;
            model.MinWallet = product != null ? Convert.ToInt32(product.MinimumCustomerEnteredPrice) : 0;
            model.MaxAmount = product != null ? Convert.ToInt32(product.MaximumCustomerEnteredPrice) : 0;
            model.PaymentSystemName = settings.PaymentSystemName;
            model.WalletProductId = settings.WalletProductId;

            var customer = await _workContext.GetCurrentCustomerAsync();
            model.HideGeneralBlock = await _genericAttributeService.GetAttributeAsync<bool>(customer, WalletDefaults.HideGeneralBlock);

            return View("~/Plugins/NopShop.Plugin.Wallet/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<WalletSettings>(storeId);

            var product = await _productService.GetProductByIdAsync(model.WalletProductId);

            settings.WalletProductId = model.WalletProductId;
            await _settingService.SaveSettingAsync(settings, settings => settings.WalletProductId, clearCache: false);
            settings.Amount = model.Amount;
            await _settingService.SaveSettingAsync(settings, settings => settings.Amount, clearCache: false);
            settings.PaymentSystemName = model.PaymentSystemName;
            await _settingService.SaveSettingAsync(settings, settings => settings.PaymentSystemName, clearCache: false);

            product.MinimumCustomerEnteredPrice = model.MinWallet;
            product.MaximumCustomerEnteredPrice = model.MaxAmount;
            await _productService.UpdateProductAsync(product);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> Charge(int amount, string paymentName)
        {
            paymentName = "Payments.BankPasargad";
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!_rewardPointsSettings.Enabled)
                return RedirectToRoute("CustomerInfo");

            var product = await _productService.GetProductByIdAsync(_walletSettings.WalletProductId);
            if (product == null)
                return RedirectToRoute("CustomerRewardPoints");

            var model = await _orderModelFactory.PrepareCustomerRewardPointsAsync(1);

            if (amount < product.MinimumCustomerEnteredPrice)
                ModelState.AddModelError(nameof(amount), $"حداقل مبلغ باید بیشتر از {await _priceFormatter.FormatPriceAsync(product.MinimumCustomerEnteredPrice, true, false)} باشد");

            if (amount > product.MaximumCustomerEnteredPrice)
                ModelState.AddModelError(nameof(amount), $"حداکثر مبلغ باید کمتر از {await _priceFormatter.FormatPriceAsync(product.MaximumCustomerEnteredPrice, true, false)} باشد");

            var addresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
            if (!addresses.Any())
                ModelState.AddModelError(nameof(amount), "ابتدا از بخش آدرس ها ، یک آدرس ثبت نمایید");

            if (!ModelState.IsValid)
                return View("~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml", model);

            if (!customer.BillingAddressId.HasValue)
                customer.BillingAddressId = addresses.First().Id;

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            foreach (var item in cart)
                await _shoppingCartService.DeleteShoppingCartItemAsync(item);

            //now let's try adding product to the cart (now including product attribute validation, etc)
            var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                product: product,
                shoppingCartType: ShoppingCartType.ShoppingCart,
                storeId: store.Id,
                attributesXml: string.Empty,
                customerEnteredPrice: amount,
                quantity: 1);
            if (addToCartWarnings.Any())
            {
                foreach (var warning in addToCartWarnings)
                    ModelState.AddModelError(nameof(amount), warning);

                return View("~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml", model);
            }
            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentName, customer, store.Id);
            if (paymentMethod == null)
            {
                ModelState.AddModelError(nameof(amount), "روش پرداخت فعال نیست");
                return View("~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml", model);
            }

            var paymentMethodInst = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentName, customer, store.Id);


            if (!_paymentPluginManager.IsPluginActive(paymentMethodInst))
            {
                ModelState.AddModelError(nameof(amount), "روش پرداخت فعال نیست");
                return View("~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml", model);
            }



            var warnings = await paymentMethod.ValidatePaymentFormAsync(null);
            if (warnings.Any())
            {
                foreach (var warning in warnings)
                    ModelState.AddModelError(nameof(amount), warning);

                return View("~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml", model);
            }

            //get payment info
            var paymentInfo = await paymentMethod.GetPaymentInfoAsync(null);
            //set previous order GUID(if exists)
            _paymentService.GenerateOrderGuid(paymentInfo);

            try
            {
                return await ConfirmOrder(false, paymentInfo, paymentName);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(amount), ex.Message);

                return View("~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml", model);
            }
        }


        public virtual async Task<IActionResult> ConfirmOrder(bool captchaValid, ProcessPaymentRequest processPaymentRequest, string selectedPaymentMethod)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                return RedirectToRoute("ShoppingCart");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);

            var isCaptchaSettingEnabled = await _customerService.IsGuestAsync(customer) &&
                _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests;
            try
            {
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                        return RedirectToRoute("CheckoutPaymentInfo");

                    processPaymentRequest = new ProcessPaymentRequest();
                }
                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.CustomerId = customer.Id;

                processPaymentRequest.PaymentMethodSystemName = selectedPaymentMethod;
                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };
                    await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        //redirection or POST has been done in PostProcessPayment
                        return Content(await _localizationService.GetResourceAsync("Checkout.RedirectMessage"));
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    throw new Exception(error);

            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc);
                model.Warnings.Add(exc.Message);

                throw new Exception(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        public virtual async Task<IActionResult> Completed(int? orderId)
        {
            //validation
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            Order order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = await _orderService.GetOrderByIdAsync(orderId.Value);
            }
            if (order == null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                order = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1))
                    .FirstOrDefault();
            }
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
            {
                return RedirectToRoute("Homepage");
            }

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            if (orderId.HasValue)
            {
                var items = await _orderService.GetOrderItemsAsync(orderId.Value);
                foreach (var item in items)
                {
                    var product = await _orderService.GetProductByOrderItemIdAsync(item.Id);
                    if (product.Id == _walletSettings.WalletProductId)
                        return RedirectToRoute("CustomerRewardPoints");
                }
            }

            return RedirectToRoute("NopCheckoutCompleted", new { orderId = order.Id });
        }


        //My account / Order details page
        public virtual async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            var items = await _orderService.GetOrderItemsAsync(orderId);
            foreach (var item in items)
            {
                var product = await _orderService.GetProductByOrderItemIdAsync(item.Id);
                if (product.Id == _walletSettings.WalletProductId)
                    return RedirectToRoute("CustomerRewardPoints");
            }

            return RedirectToRoute("NopOrderDetails", new { orderId = order.Id });
        }
    }
}