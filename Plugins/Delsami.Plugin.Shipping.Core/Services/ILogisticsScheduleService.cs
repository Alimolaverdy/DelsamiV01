using ZarIran.Plugin.Shipping.Core.Domain;
using Nop.Core;
using System;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Core.Services
{
    public interface ILogisticsScheduleService
    {
        Task<LogisticsSchedule> GetLogisticsScheduleAsync(int id);

        Task<bool> IsExistDayOfWeek(DayOfWeek dayOfWeek, int? currentLogiscticsScheduleId = null);

        Task InsertAsync(LogisticsSchedule logisticsScheduleInfo);

        Task UpdateAsync(LogisticsSchedule logisticsScheduleInfo);

        Task DeleteAsync(LogisticsSchedule logisticsScheduleInfo);

        Task<IPagedList<LogisticsSchedule>> GetLogisticsSchedulesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<LogisticsSchedule>> GetLogisticsSchedulesByZarIranEventAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<bool> IsLogisticsActiveAsync(DateTime dateTime);

        Task<bool> IsLogisticsActiveForScheduleAsync(DateTime dateTime, TimeSpan startHour, TimeSpan endHour);
    }
}
