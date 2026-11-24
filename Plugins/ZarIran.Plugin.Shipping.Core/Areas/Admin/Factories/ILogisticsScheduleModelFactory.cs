using ZarIran.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedules;
using ZarIran.Plugin.Shipping.Core.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Core.Areas.Admin.Factories
{
    public interface ILogisticsScheduleModelFactory
    {
        LogisticsScheduleSearchModel PrepareLogisticsScheduleSearchModel(LogisticsScheduleSearchModel searchModel);

        LogisticsScheduleModel PrepareLogisticsScheduleModel(LogisticsScheduleModel model, LogisticsSchedule carton);

        Task<LogisticsScheduleListModel> PrepareLogisticsScheduleListModelAsync(LogisticsScheduleSearchModel searchModel);
    }
}
