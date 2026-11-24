using Nop.Data.Mapping;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(AvailableDeliveryDateTimeRange), "ZI_DS_AvailableDeliveryDateTimeRange"},
            { typeof(AvailableDeliveryTimeRange), "ZI_DS_AvailableDeliveryTimeRange"},
            { typeof(ExceptionDate), "ZI_DS_ExceptionDate"},
            { typeof(OrderShippingMethodCapacityMapping), "ZI_DS_OrderShippingMethodCapacityMapping"},
            { typeof(OverriddenStartDisplayDaysOffset), "ZI_DS_OverriddenStartDisplayDaysOffset"},
            { typeof(ShippingMethodCapacity), "ZI_DS_ShippingMethodCapacity"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}