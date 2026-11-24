using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data.Mapping;

public class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
    {
        { typeof(PaymentMethodCityRestriction), $"ZI_MP_{nameof(PaymentMethodCityRestriction)}"},
        { typeof(PaymentMethodCustomerRoleRestriction), $"ZI_MP_{nameof(PaymentMethodCustomerRoleRestriction)}"},
        { typeof(PaymentMethodDiscount), $"ZI_MP_{nameof(PaymentMethodDiscount)}"},
        { typeof (PaymentMethodRewardPoint),$"ZI_MP_{nameof(PaymentMethodRewardPoint)}" },
        { typeof (PaymentMethodAmountRestriction),$"ZI_MP_{nameof(PaymentMethodAmountRestriction)}" },
        { typeof (PaymentMethodShippingRestriction),$"ZI_MP_{nameof(PaymentMethodShippingRestriction)}" },
    };

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
}
