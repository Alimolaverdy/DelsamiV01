using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public interface IExtendedOrderService
    {
        Task<Order> GetCurrentCustomerLastOrderAsync();

        Task<List<Order>> GetExceptDelsamiLogisticProcessingOrdersAfterThreeDaysAsync();

        Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
         int vendorId = 0, int customerId = 0,
         int productId = 0, int affiliateId = 0, int warehouseId = 0, List<int> ShippingMethodIds = null,
         int billingCountryId = 0, string paymentMethodSystemName = null,
         DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
         List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
         string billingPhone = null, string billingEmail = null, string billingLastName = "",
         string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);


        Task<IPagedList<Order>> GetOrdersAsync(DateTime paidDate, int limit);

        Task<(List<OrderItem> OrderItems, Dictionary<int, Product> ProductsDic)> GetProductsOrderItemsByOrderIdsAsync(List<int> orderIds);

        Task<List<OrderItem>> GetOrderItemsByOrderIdsAsync(List<int> orderIds);

    }
}
