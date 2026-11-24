using ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Models.Shipments;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Factories
{
    public interface IZarIranShipmentModelFactory
    {
        Task<ZarIranOrderShipmentListModel> PrepareOrderShipmentListModelAsync(OrderShipmentSearchModel searchModel, Order order);
    }
}
