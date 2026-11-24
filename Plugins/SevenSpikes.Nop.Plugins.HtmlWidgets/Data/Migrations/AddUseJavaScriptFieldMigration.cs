using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Table;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Data.Mapping.Migrations;

public class AddUseJavaScriptFieldMigration : AutoReversingMigration
{
    public override void Up()
    {
        if (!Schema.Table("SS_HW_HtmlWidget").Column("UseJavaScript").Exists())
        {
            ((IColumnOptionSyntax<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax, IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax>)((IColumnTypeSyntax<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax>)Alter.Table("SS_HW_HtmlWidget").AddColumn("UseJavaScript")).AsBoolean()).WithDefaultValue(false);
        }
        if (!Schema.Table("SS_HW_HtmlWidget").Column("JavaScriptContent").Exists())
        {
            ((IColumnOptionSyntax<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax, IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax>)((IColumnTypeSyntax<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax>)Alter.Table("SS_HW_HtmlWidget").AddColumn("JavaScriptContent")).AsString()).Nullable();
        }
    }
}
