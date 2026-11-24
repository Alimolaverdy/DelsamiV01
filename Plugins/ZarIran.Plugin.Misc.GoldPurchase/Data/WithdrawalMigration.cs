using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Data;

[NopMigration("2026/11/23 12:00:00", "Gold purchase withdrawal requests")]
public class WithdrawalMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<GoldWithdrawal>();
        Create.TableFor<GoldOrderWalletPayment>();
    }
}
