namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Validator;

using FluentValidation;
using System.Linq;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.AmountRestriction;


public class AmountRestrictionValidator : AbstractValidator<PaymentMethodAmountRestrictionList>
{
    public AmountRestrictionValidator()
    {
        RuleFor(x => x.PaymentMethods)
            .Must(methods => methods.All(m => m.MinAmount == null || m.MinAmount >= 0))
            .WithMessage("حداقل مقدار نمی‌تواند کمتر از صفر باشد");

        RuleFor(x => x.PaymentMethods)
            .Must(methods => methods.All(m =>
                m.MaxAmount == null ||
                m.MinAmount == null ||
                m.MaxAmount == 0 ||
                m.MinAmount == 0 ||
                m.MaxAmount >= m.MinAmount))
            .WithMessage("حداکثر مقدار نباید از حداقل مقدار کوچکتر باشد");
    }
}
