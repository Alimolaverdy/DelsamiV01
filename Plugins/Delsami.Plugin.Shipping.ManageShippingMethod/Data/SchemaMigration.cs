using FluentMigrator;
using Delsami.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Data
{
    [NopMigration("2026/11/26 11:00:00", "Delsami.Plugin.Shipping.ManageShippingMethod base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ShippingMethodCityRestriction>();
        }
    }
}
