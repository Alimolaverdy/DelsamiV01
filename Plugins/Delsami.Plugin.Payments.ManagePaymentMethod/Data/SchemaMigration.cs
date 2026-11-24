using FluentMigrator;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data
{
    [NopMigration("2026/06/16 09:38:00", "ZarIran.Plugin.Payment.ManagePaymentMethod base schema, add PaymentMethodShippingRestriction")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<PaymentMethodCityRestriction>();
            Create.TableFor<PaymentMethodCustomerRoleRestriction>();
            Create.TableFor<PaymentMethodDiscount>();
            Create.TableFor<PaymentMethodRewardPoint>();
            Create.TableFor<PaymentMethodAmountRestriction>();
            Create.TableFor<PaymentMethodShippingRestriction>();
        }
    }
}
