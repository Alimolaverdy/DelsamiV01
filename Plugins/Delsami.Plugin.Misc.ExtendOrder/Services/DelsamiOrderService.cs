using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Html;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services;

public class DelsamiOrderService : OrderService
{
    private readonly IWorkContext _workContext;
    public DelsamiOrderService(IWorkContext workContext,
     IHtmlFormatter htmlFormatter,
     IProductService productService,
     IRepository<Address> addressRepository,
     IRepository<Customer> customerRepository,
     IRepository<Order> orderRepository,
     IRepository<OrderItem> orderItemRepository,
     IRepository<OrderNote> orderNoteRepository,
     IRepository<Product> productRepository,
     IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
     IRepository<RecurringPayment> recurringPaymentRepository,
     IRepository<RecurringPaymentHistory> recurringPaymentHistoryRepository,
     IShipmentService shipmentService) : base(htmlFormatter,
                                            productService,
                                            addressRepository,
                                            customerRepository,
                                            orderRepository,
                                            orderItemRepository,
                                            orderNoteRepository,
                                            productRepository,
                                            productWarehouseInventoryRepository,
                                            recurringPaymentRepository,
                                            recurringPaymentHistoryRepository,
                                            shipmentService)
    {
        _workContext = workContext;
    }


    public override async Task InsertOrderNoteAsync(OrderNote orderNote)
    {
        if (!orderNote.DisplayToCustomer)
        {

            var orginal = _workContext.OriginalCustomerIfImpersonated;
            Customer customer = null;

            if (orginal is null)
            {
                customer = await _workContext.GetCurrentCustomerAsync();
            }
            else
            {
                customer = orginal;

            }
            if (!string.IsNullOrEmpty(customer.FirstName) || !string.IsNullOrEmpty(customer.LastName))
                orderNote.Note = $"{orderNote.Note} - ({customer.FirstName} {customer.LastName})";
        }


        await base.InsertOrderNoteAsync(orderNote);
    }



}
