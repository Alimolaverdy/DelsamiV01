using System;
using Nop.Data.Migrations;
using SevenSpikes.Nop.Framework.Migrations;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Data.Mapping.Migrations;

public class SchemaMigration : Base7SpikesSchemaMigration
{
    public SchemaMigration(IMigrationManager migrationManager)
        : base(migrationManager)
    {
    }

    public override void Up()
    {
        CreateOrUpdateTableIfExist<HtmlWidget>("SS_HW_HtmlWidget", Array.Empty<ForeignKeyInfo>());
    }

    public override void Down()
    {
        DeleteTablesIfExist(new[] { "SS_HW_HtmlWidget" });
    }
}
