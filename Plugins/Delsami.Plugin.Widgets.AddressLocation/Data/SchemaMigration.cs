using FluentMigrator;
using ZarIran.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Widgets.AddressLocation.Data
{
    [NopMigration("2025/07/07 09:38:00", "ZarIran.Plugin.Widgets.AddressLocation base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<StateProvinceLocation>();
            Create.TableFor<AddressCity>();
        }
    }
}
