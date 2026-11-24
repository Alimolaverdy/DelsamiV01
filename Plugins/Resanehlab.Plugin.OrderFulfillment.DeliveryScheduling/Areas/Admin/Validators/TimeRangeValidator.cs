using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Validators
{
    public class TimeRangeValidator : BaseNopValidator<TimeRangeModel>
    {
        public TimeRangeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Capacity).NotNull().WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeRange.Fields.Capacity.Required"));
        }
    }
}
