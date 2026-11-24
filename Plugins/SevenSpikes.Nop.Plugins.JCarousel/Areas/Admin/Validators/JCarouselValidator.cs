using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Models;

namespace SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Validators;

public class JCarouselValidator : AbstractValidator<JCarouselSettingsModel>
{
    public JCarouselValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.Name)
            .NotNull()
            .WithMessageAwait(localizationService.GetResourceAsync(
                "SevenSpikes.JCarousel.Admin.Settings.Name.Required"));

        RuleFor(model => model.JCarouselEntitySettings.ProductImageSize)
            .GreaterThanOrEqualTo(0)
            .WithMessageAwait(localizationService.GetResourceAsync(
                "SevenSpikes.JCarousel.Admin.Settings.ProductImageSize.Error.PositiveNumber"));
    }
}
