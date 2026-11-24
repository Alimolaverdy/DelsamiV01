using ZarIran.Plugin.Shipping.ZarIranLogistic.Models;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Services
{
    public interface IZarIranLogisticApiService
    {
        Task<CloseCargoAndOrderBySellSiteResultModel> CloseCargoAndOrderBySellSiteAsync(int shipmentId);
    }
}
