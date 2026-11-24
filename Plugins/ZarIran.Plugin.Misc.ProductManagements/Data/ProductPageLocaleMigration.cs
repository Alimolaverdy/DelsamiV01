using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace ZarIran.Plugin.Misc.ProductManagements.Data;

[NopMigration("2026/11/18 12:45:00", "Add product page language resources")]
public class ProductPageLocaleMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        EnsureResources(EngineContext.Current.Resolve<ILanguageService>(),
            EngineContext.Current.Resolve<ILocalizationService>());
    }

    internal static void EnsureResources(ILanguageService languageService, ILocalizationService localizationService)
    {
        foreach (var language in languageService.GetAllLanguages(showHidden: true))
        {
            var persian = string.Equals(language.LanguageCulture, "fa", StringComparison.OrdinalIgnoreCase) ||
                (language.LanguageCulture?.StartsWith("fa-", StringComparison.OrdinalIgnoreCase) ?? false);
            var defaults = new Dictionary<string, string>
            {
                ["shippment-2hours-tehran"] = persian ? "ارسال در تهران" : "Delivery in Tehran",
                ["shippment-fast-everywhere"] = persian ? "ارسال سریع به سراسر کشور" : "Fast nationwide delivery",
                ["product-warranty"] = persian ? "ضمانت محصول" : "Product warranty",
                ["products.guarantee"] = persian ? "ضمانت" : "Guarantee",
                ["ZarIran.Products.FullDescription"] = persian ? "توضیحات محصول" : "Product description"
            };

            var missingResources = defaults
                .Where(resource => localizationService.GetLocaleStringResourceByName(resource.Key, language.Id, false) == null)
                .ToDictionary(resource => resource.Key, resource => resource.Value);

            if (missingResources.Count > 0)
                localizationService.AddOrUpdateLocaleResource(missingResources, language.Id);
        }
    }

    public override void Down()
    {
        // Keep language values that store owners may have edited.
    }
}
