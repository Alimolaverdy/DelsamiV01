using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Data
{
    [NopMigration("2026/11/14 14:35:20", "ZarIran.Plugin.Misc.GoldPurchase base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<GoldWallet>();
            Create.TableFor<GoldTransaction>();
        }
    }
}
