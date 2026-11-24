using FluentValidation;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Validators
{
    public class CustomerAttributeValidationValidator : BaseNopValidator<CustomerAttributeValidationModel>
    {
        public CustomerAttributeValidationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CustomerAttributeId).NotEqual(0).WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId.IsRequired"));

            When(x => !x.IsNationalCode, () =>
            {
                RuleFor(x => x.RegexPattern).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern.IsRequired"));
            });
        }
    }
}