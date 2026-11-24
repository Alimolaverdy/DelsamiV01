using System;
using Nop.Data.Migrations;
using SevenSpikes.Nop.Framework.Migrations;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;

namespace SevenSpikes.Nop.Plugins.JCarousel.Data.Migrations;

public class SchemaMigration : Base7SpikesSchemaMigration
{
    public SchemaMigration(IMigrationManager migrationManager)
        : base(migrationManager)
    {
    }

    public override void Up()
    {
        CreateOrUpdateTableIfExist<Carousel>("SS_JC_JCarousel", Array.Empty<ForeignKeyInfo>());
    }

    public override void Down()
    {
        DeleteTablesIfExist("SS_JC_JCarousel");
    }
}
