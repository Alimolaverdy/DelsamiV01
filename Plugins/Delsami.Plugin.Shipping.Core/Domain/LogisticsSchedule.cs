using Nop.Core;
using System;

namespace ZarIran.Plugin.Shipping.Core.Domain
{
    public class LogisticsSchedule : BaseEntity
    {
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        //public DateTime? ValidFrom { get; set; }

        //public DateTime? ValidTo { get; set; }
    }
}
