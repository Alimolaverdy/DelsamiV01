using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Data.Mapping.Builder
{
    internal class PaymentMethodRewardPointBuilder : NopEntityBuilder<PaymentMethodRewardPoint>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodRewardPoint.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentMethodRewardPoint.RewardPointHistoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentMethodRewardPoint.Status)).AsString().NotNullable();
        }
    }
}
