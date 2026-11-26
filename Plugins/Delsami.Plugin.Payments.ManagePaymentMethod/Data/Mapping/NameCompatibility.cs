using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Data.Mapping;

public class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
    {
        { typeof(PaymentMethodCityRestriction), $"D_MP_{nameof(PaymentMethodCityRestriction)}"},
        { typeof(PaymentMethodCustomerRoleRestriction), $"D_MP_{nameof(PaymentMethodCustomerRoleRestriction)}"},
        { typeof(PaymentMethodDiscount), $"D_MP_{nameof(PaymentMethodDiscount)}"},
        { typeof (PaymentMethodRewardPoint),$"D_MP_{nameof(PaymentMethodRewardPoint)}" },
        { typeof (PaymentMethodAmountRestriction),$"D_MP_{nameof(PaymentMethodAmountRestriction)}" },
        { typeof (PaymentMethodShippingRestriction),$"D_MP_{nameof(PaymentMethodShippingRestriction)}" },
    };

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
}
