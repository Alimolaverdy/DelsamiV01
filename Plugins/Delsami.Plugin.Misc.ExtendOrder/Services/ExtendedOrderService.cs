using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public class ExtendedOrderService : IExtendedOrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderShippingMethod> _orderShippingMethodRepo;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IWorkContext _workContext;
        private readonly IShipmentService _shipmentService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Address> _addressRepository;


        public ExtendedOrderService(IRepository<Order> orderRepo,
                                    IWorkContext workContext,
                                    IRepository<OrderShippingMethod> orderShippingMethodRepo,
                                    IRepository<OrderItem> orderItemRepository,
                                    IRepository<Product> productRepository,
                                    IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
                                    IRepository<OrderNote> orderNoteRepository,
                                    IRepository<Address> addressRepository,
                                    IShipmentService shipmentService)
        {
            _orderRepo = orderRepo;
            _orderShippingMethodRepo = orderShippingMethodRepo;
            _workContext = workContext;
            _shipmentService = shipmentService;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _orderNoteRepository = orderNoteRepository;
            _addressRepository = addressRepository;
        }


        public async Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
                 int vendorId = 0, int customerId = 0,
                 int productId = 0, int affiliateId = 0, int warehouseId = 0, List<int> shippingMethodIds = null,
                 int billingCountryId = 0, string paymentMethodSystemName = null,
                 DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
                 List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
                 string billingPhone = null, string billingEmail = null, string billingLastName = "",
                 string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _orderRepo.Table;

            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);

            if (shippingMethodIds != null && shippingMethodIds.Any() && shippingMethodIds.First() > 0)
            {
                var shippingQuery = _orderShippingMethodRepo.Table;

                query = from sh in shippingQuery
                        where shippingMethodIds.Contains(sh.ShippingMethodId)
                        join o in query on sh.OrderId equals o.Id
                        select o;
            }

            if (vendorId > 0)
            {
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        where p.VendorId == vendorId
                        select o;

                query = query.Distinct();
            }

            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (productId > 0)
            {
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        where oi.ProductId == productId
                        select o;

                query = query.Distinct();
            }

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId into ps
                        from pwi in ps.DefaultIfEmpty()
                        where
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (p.ManageInventoryMethodId == manageStockInventoryMethodId && p.UseMultipleWarehouses && pwi.WarehouseId == warehouseId) ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((p.ManageInventoryMethodId != manageStockInventoryMethodId || !p.UseMultipleWarehouses) && p.WarehouseId == warehouseId)
                        select o;

                query = query.Distinct();
            }

            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));

            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => _orderNoteRepository.Table.Any(oNote => oNote.OrderId == o.Id && oNote.Note.Contains(orderNotes)));

            query = from o in query
                    join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                    where
                        (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                        (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                        (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                        (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))
                    select o;

            query = query.Where(o => !o.Deleted);

            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }


        public async Task<Order> GetCurrentCustomerLastOrderAsync()
        {
            var query = _orderRepo.Table.AsQueryable();

            EngineContext.Current.Resolve<ILogger>().Information($"GetCurrentCustomerAsync {DateTime.Now}");

            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            EngineContext.Current.Resolve<ILogger>().Information($"GetCurrentCustomerAsync {DateTime.Now}");

            var lastOrder = query.Where(w => w.CustomerId == currentCustomer.Id && w.OrderStatusId == (int)OrderStatus.Pending).OrderByDescending(o => o.Id).FirstOrDefault();

            return lastOrder;
        }

        public async Task<List<Order>> GetExceptDelsamiLogisticProcessingOrdersAfterThreeDaysAsync()
        {
            var orders = await _orderRepo.Table.Where(w => w.OrderStatusId == (int)OrderStatus.Processing && w.ShippingRateComputationMethodSystemName != "Delsami.Shipping.DelsamiLogistic").ToListAsync();
            List<Order> selectedOrders = new();

            foreach (var order in orders)
            {
                var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);
                var shipment = shipments.FirstOrDefault(a => a.ShippedDateUtc.HasValue);

                if (shipment is not null && (int)(DateTime.UtcNow - shipment.ShippedDateUtc.Value).TotalDays == 3)
                    selectedOrders.Add(order);
            }

            return selectedOrders;
        }

        public async Task<IPagedList<Order>> GetOrdersAsync(DateTime paidDate, int limit)
        {

            var query = _orderRepo.Table.Where(a => a.PaidDateUtc.HasValue && a.PaidDateUtc >= paidDate.ToUniversalTime()
                                               && !a.Deleted
                                               && !string.IsNullOrEmpty(a.CardName)
                                               && (a.OrderStatusId == (int)OrderStatus.Complete || a.OrderStatusId == (int)OrderStatus.Processing || a.OrderStatusId == (int)OrderStatus.Cancelled));

            return await query.OrderBy(a => a.CreatedOnUtc).ToPagedListAsync(0, pageSize: limit);

        }

        public async Task<(List<OrderItem> OrderItems, Dictionary<int, Product> ProductsDic)> GetProductsOrderItemsByOrderIdsAsync(List<int> orderIds)
        {
            var query = from orderItem in _orderItemRepository.Table.Where(a => orderIds.Contains(a.OrderId))
                        join simpleProduct in _productRepository.Table.Where(w => w.ProductTypeId == (int)ProductType.SimpleProduct) on orderItem.ProductId equals simpleProduct.Id
                        join groupedProduct in _productRepository.Table.Where(w => w.ProductTypeId == (int)ProductType.GroupedProduct) on simpleProduct.ParentGroupedProductId equals groupedProduct.Id
                        select new { orderItem, simpleProduct.Id, groupedProduct };

            var result = await query.ToListAsync();


            return (
                result.Select(x => x.orderItem).ToList(),
                result.GroupBy(g => g.Id).ToDictionary(x => x.Key, x => x.First().groupedProduct)
            );
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdsAsync(List<int> orderIds)
        {
            var query = from orderItem in _orderItemRepository.Table.Where(a => orderIds.Contains(a.Id))
                        select orderItem;

            return await query.ToListAsync();
        }
    }
}
