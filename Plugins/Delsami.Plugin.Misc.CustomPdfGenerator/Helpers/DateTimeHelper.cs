using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Helpers
{
    public class DateTimeHelper
    {
        private static PersianCalendar _pc = new();

        #region Methods

        public static int GetPersianYear(DateTime dateTime)
        {
            return _pc.GetYear(dateTime);
        }

        public static DateTime? GetDateTimeFromPersian(string date, string seperator = "/")
        {
            if (string.IsNullOrWhiteSpace(date))
                return null;

            var splittedDate = date.Split("/");

            return GetDateTimeFromPersian(int.Parse(splittedDate[0]), int.Parse(splittedDate[1]), int.Parse(splittedDate[2]));
        }

        public static DateTime GetDateTimeFromPersian(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int milliSecond = 0)
        {
            return _pc.ToDateTime(year, month, day, hour, minute, second, milliSecond);
        }

        public static string GetPersianStrFromDateTime(DateTime? dateTime, bool showTime = false)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            var value = $"{_pc.GetYear(dateTime.Value)}/{_pc.GetMonth(dateTime.Value)}/{_pc.GetDayOfMonth(dateTime.Value)}";

            if (showTime)
                value += $"-{dateTime.Value.ToString("HH:mm:ss")}";

            return value;

        }

        public static int GetYearFromPersian(string date, string seperator = "/")
        {
            //برای تبدیل تاریخ عجیب غریب سرویس صندوق 
            if (string.IsNullOrEmpty(date))
                return 0;

            var splittedDate = date.Split("/");

            return int.Parse(splittedDate[0]);

        }

        #endregion
    }
}
