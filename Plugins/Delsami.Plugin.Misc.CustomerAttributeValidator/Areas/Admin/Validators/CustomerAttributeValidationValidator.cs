using FluentValidation;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Validators
{
    public class CustomerAttributeValidationValidator : BaseNopValidator<CustomerAttributeValidationModel>
    {
        public CustomerAttributeValidationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CustomerAttributeId).NotEqual(0).WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId.IsRequired"));

            When(x => !x.IsNationalCode, () =>
            {
                RuleFor(x => x.RegexPattern).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern.IsRequired"));
            });
        }
    }
}