using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Enums;
using ZarIran.Plugin.Misc.Core.Events;
using ZarIran.Plugin.Misc.Core.Helpers;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Seo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Services;

namespace ZarIran.Plugin.Misc.ProductManagements.EventHandlers
{
    public class BackInStockSubscriptionEventHandler : IConsumer<EntityInsertedEvent<StockQuantityHistory>>
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IZarIranBackInStockSubscriptionService _zarIranIBackInStockSubscriptionService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CoreSettings _coreSettings;
        private readonly IProductStockHistoryService _productStockHistoryService;

        #endregion

        #region Ctor

        public BackInStockSubscriptionEventHandler(IProductService productService,
                                                   IZarIranBackInStockSubscriptionService zarIranIBackInStockSubscriptionService,
                                                   ICustomerService customerService,
                                                   IUrlRecordService urlRecordService,
                                                   IWebHelper webHelper,
                                                   IVirtualProductMappingService virtualProductMappingService,
                                                   ILocalizationService localizationService,
                                                   IEventPublisher eventPublisher,
                                                   CoreSettings coreSettings,
                                                   IProductStockHistoryService productStockHistoryService)
        {
            _productService = productService;
            _zarIranIBackInStockSubscriptionService = zarIranIBackInStockSubscriptionService;
            _customerService = customerService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _virtualProductMappingService = virtualProductMappingService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _coreSettings = coreSettings;
            _productStockHistoryService = productStockHistoryService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(EntityInsertedEvent<StockQuantityHistory> eventMessage)
        {
            var stockHistory = eventMessage?.Entity;
            if (stockHistory == null)
                return;

            var product = await _productService.GetProductByIdAsync(stockHistory.ProductId);
            if (product == null)
                return;


            var previousStockQuantity = await _productStockHistoryService.GetPreviousStockQuantityAsync(product);
            var currentStockQuantity = stockHistory.StockQuantity;

            if (previousStockQuantity <= 0 && currentStockQuantity > 0)
            {
                var virtualProducts = await _virtualProductMappingService.GetVirtualProductMappingsByProductIdAsync(stockHistory.ProductId);

                List<int> productIds = new() { stockHistory.ProductId };

                if (virtualProducts.Any())
                    productIds.AddRange(virtualProducts.Select(s => s.VirtualProductId));

                var subscriptions = await _zarIranIBackInStockSubscriptionService.GetAllValidForNotifiedByProductIdsAsync(productIds);

                if (subscriptions.Any())
                {
                    foreach (var subscription in subscriptions)
                    {
                        var fullName = "";
                        var customer = await _customerService.GetCustomerByUsernameAsync(subscription.MobileNumber);

                        if (customer != null && !string.IsNullOrWhiteSpace(customer.FirstName) && !string.IsNullOrWhiteSpace(customer.LastName))
                            fullName = $"{customer.FirstName} {customer.LastName}";
                        else
                            fullName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.BackInStockSubscription.NoNameCustomer");

                        var backInStockSubscription = new BackInStockSubscriptionTriggeredEvent
                        {
                            MobilePhoneNumber = subscription.MobileNumber,
                            FullName = fullName,
                            ProductName = product.Name,
                            ProductUrl = SeNameHelper.GenerateSeName(SeNameType.Product, await _urlRecordService.GetSeNameAsync(product))
                        };

                        await _eventPublisher.PublishAsync(backInStockSubscription);

                        await _zarIranIBackInStockSubscriptionService.NotifiedAsync(subscription);
                    }
                }
            }
        }

        #endregion
    }
}