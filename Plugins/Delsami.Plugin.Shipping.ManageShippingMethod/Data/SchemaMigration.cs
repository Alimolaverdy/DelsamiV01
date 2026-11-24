using FluentMigrator;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Data
{
    [NopMigration("2025/03/02 11:00:00", "ZarIran.Plugin.Shipping.ManageShippingMethod base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ShippingMethodCityRestriction>();
        }
    }
}
