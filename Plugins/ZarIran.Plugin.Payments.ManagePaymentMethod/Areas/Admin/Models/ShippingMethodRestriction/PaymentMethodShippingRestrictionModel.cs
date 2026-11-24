using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.ShippingMethodRestriction
{
    public record PaymentMethodShippingRestrictionModel : BaseNopModel
    {
        #region Ctor

        public PaymentMethodShippingRestrictionModel()
        {
            AvailablePaymentMethods = new List<PaymentMethodModel>();
            AvailableShippingMethods = new List<ShippingMethodModel>();
            Restricted = new Dictionary<int, IDictionary<string, bool>>();
        }

        #endregion

        #region Properties

        public IList<PaymentMethodModel> AvailablePaymentMethods { get; set; }

        public IList<ShippingMethodModel> AvailableShippingMethods { get; set; }

        //[shipping method systemName] / [payment method id] / [restricted]
        public IDictionary<int, IDictionary<string, bool>> Restricted { get; set; }

        #endregion
    }
}
