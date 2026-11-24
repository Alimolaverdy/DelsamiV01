using System.Collections.Generic;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using SevenSpikes.Nop.EntitySettings.Services.Install;
using SevenSpikes.Nop.Framework.Domain.Enums;
using SevenSpikes.Nop.Framework.Migrations;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.JCarousel.Services;

namespace SevenSpikes.Nop.Plugins.JCarousel.Data.Migrations;

public class AddMissingEntitySettingsMigration : Base7SpikesSchemaMigration
{
    protected IEntitySettingsInstallerService _entitySettingsInstallerService;

    public AddMissingEntitySettingsMigration(IMigrationManager migrationManager, IEntitySettingsInstallerService entitySettingsInstallerService)
        : base(migrationManager)
    {
        _entitySettingsInstallerService = entitySettingsInstallerService;
    }

    public override void Down()
    {
        _entitySettingsInstallerService.UnInstallEntitySettingsForEntityAsync(EntityType.Carousel).GetAwaiter().GetResult();
    }

    public override void Up()
    {
        IList<Carousel> result = EngineContext.Current.Resolve<IJCarouselService>().GetAllCarouselsAsync().GetAwaiter().GetResult();
        if (result.Count > 0)
        {
            _entitySettingsInstallerService.InstallDefaultSettingsForEntitiesAsync<JCarouselEntitySettings, Carousel>(result, Plugin.EntityType).GetAwaiter().GetResult();
        }
    }
}
