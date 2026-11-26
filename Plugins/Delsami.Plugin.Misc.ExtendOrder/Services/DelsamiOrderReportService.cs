using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public class DelsamiOrderReportService: IDelsamiOrderReportService
    {
        #region Fields

        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;

        #endregion

        #region Ctor

        public DelsamiOrderReportService(IRepository<Order> orderRepository,
                                          IRepository<OrderItem> orderItemRepository,
                                          IRepository<Product> productRepository,
                                          IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
                                          IRepository<Address> addressRepository,
                                          IRepository<OrderNote> orderNoteRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _addressRepository = addressRepository;
            _orderNoteRepository = orderNoteRepository;
        }

        #endregion

        #region Methods

        public virtual async Task<DelsamiOrderAverageReportLine> GetOrderAverageReportLineAsync(int storeId = 0,
                                                                                         int vendorId = 0,
                                                                                         int productId = 0,
                                                                                         int warehouseId = 0,
                                                                                         int billingCountryId = 0,
                                                                                         int orderId = 0,
                                                                                         string paymentMethodSystemName = null,
                                                                                         List<int> osIds = null,
                                                                                         List<int> psIds = null,
                                                                                         List<int> ssIds = null,
                                                                                         DateTime? startTimeUtc = null,
                                                                                         DateTime? endTimeUtc = null,
                                                                                         string billingPhone = null,
                                                                                         string billingEmail = null,
                                                                                         string billingLastName = "",
                                                                                         string orderNotes = null)
        {
            var query = _orderRepository.Table;

            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderId > 0)
                query = query.Where(o => o.Id == orderId);

            if (vendorId > 0)
            {
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        where p.VendorId == vendorId
                        select o;

                query = query.Distinct();
            }

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
                        join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId
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

            query = from o in query
                    join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                    where
                        (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                        (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                        (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                        (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))
                    select o;

            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));

            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);

            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);

            if (!string.IsNullOrEmpty(orderNotes))
            {
                query = from o in query
                        join n in _orderNoteRepository.Table on o.Id equals n.OrderId
                        where n.Note.Contains(orderNotes)
                        select o;

                query.Distinct();
            }

            var item = await (from oq in query
                              group oq by 1
                into result
                              select new
                              {
                                  OrderCount = result.Count(),
                                  OrderShippingExclTaxSum = result.Sum(o => o.OrderShippingExclTax),
                                  OrderPaymentFeeExclTaxSum = result.Sum(o => o.PaymentMethodAdditionalFeeExclTax),
                                  OrderTaxSum = result.Sum(o => o.OrderTax),
                                  OrderTotalSum = result.Sum(o => o.OrderTotal),
                                  OrederRefundedAmountSum = result.Sum(o => o.RefundedAmount),
                                  OrderSubTotalExlcludeTax = result.Sum(o => o.OrderSubtotalExclTax),
                              }).Select(r => new DelsamiOrderAverageReportLine
                              {
                                  CountOrders = r.OrderCount,
                                  SumShippingExclTax = r.OrderShippingExclTaxSum,
                                  OrderPaymentFeeExclTaxSum = r.OrderPaymentFeeExclTaxSum,
                                  SumTax = r.OrderTaxSum,
                                  SumOrders = r.OrderTotalSum,
                                  SumRefundedAmount = r.OrederRefundedAmountSum,
                                  SumOrderSubtotalExcludeTaxs= r.OrderSubTotalExlcludeTax
                              })
                .FirstOrDefaultAsync();

            item ??= new DelsamiOrderAverageReportLine
            {
                CountOrders = 0,
                SumShippingExclTax = decimal.Zero,
                OrderPaymentFeeExclTaxSum = decimal.Zero,
                SumTax = decimal.Zero,
                SumOrders = decimal.Zero
            };
            return item;
        }

        #endregion
    }
}
