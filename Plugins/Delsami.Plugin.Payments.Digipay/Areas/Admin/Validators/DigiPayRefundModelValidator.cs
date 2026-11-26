using System.Globalization;
using System.Linq;
using FluentValidation;
using Delsami.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Delsami.Plugin.Payments.Digipay.Areas.Admin.Validators
{
    public class DigiPayRefundModelValidator : BaseNopValidator<DigiPayRefundModel>
    {
        public DigiPayRefundModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Amount)
				.NotEmpty().NotNull().Must(IsValidAmount)
                .WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Fields.Amount.Invalid"));

            RuleFor(x => x.PaymentTrackingCode)
                .NotEmpty().NotNull()
                .WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Fields.PaymentTrackingCode.Required"));

            RuleFor(x => x.PaymentType)
                .IsInEnum()
                .WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Fields.PaymentType.Invalid"));
        }

        private static bool IsValidAmount(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var normalized = value
                .Replace("٬", "").Replace(",", "")
                .Select(c => c >= '۰' && c <= '۹' ? (char)(c - '۰' + '0') : c)
                .Aggregate("", (s, c) => s + c);

            return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) && parsed > 0;
        }
    }
}
