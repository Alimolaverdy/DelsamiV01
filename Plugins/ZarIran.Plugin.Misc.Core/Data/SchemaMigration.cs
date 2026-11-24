using DocumentFormat.OpenXml.Bibliography;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using ZarIran.Plugin.Misc.Core.Domain;
using City = ZarIran.Plugin.Misc.Core.Domain.City;

namespace ZarIran.Plugin.Misc.Core.Data
{
    [NopMigration("2025/10/05 14:35:20", "ZarIran.Plugin.Misc.Core base schema")]
    internal class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<City>();
            Create.TableFor<OTP>();
            Create.TableFor<ImportReport>();
            Create.TableFor<VirtualProductMapping>();
        }
    }
}
