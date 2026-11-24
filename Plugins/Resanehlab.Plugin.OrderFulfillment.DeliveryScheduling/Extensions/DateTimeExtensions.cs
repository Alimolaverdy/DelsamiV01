using Nop.Core;
using Nop.Core.Infrastructure;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Extensions
{
    public static class DateTimeExtension
    {
        public static async Task<string> FormatDateTimeAsync(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return string.Empty;

            var deliverySchedulingSetting = EngineContext.Current.Resolve<DeliverySchedulingSetting>();
            var workingLanguage = await EngineContext.Current.Resolve<IWorkContext>().GetWorkingLanguageAsync();

            var cultureInfo = new CultureInfo("en-US");
            if (deliverySchedulingSetting.FormatDeliveryDateTimesBasedOnCustomerLanguage)
                cultureInfo = new CultureInfo(workingLanguage.LanguageCulture);

            if (!string.IsNullOrWhiteSpace(deliverySchedulingSetting.DeliveryDateTimeFormatString))
                return dateTime.ToString(deliverySchedulingSetting.DeliveryDateTimeFormatString, cultureInfo);

            return dateTime.ToString(Constants.DefaultDeliveryDateTimeFormatString, cultureInfo);
        }
    }
}
