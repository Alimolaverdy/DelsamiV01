using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models
{
    public record ZarIranPaymentMethodCustomerRoleRestrictionModel : BaseNopModel
    {
        #region Ctor

        public ZarIranPaymentMethodCustomerRoleRestrictionModel()
        {
            AvailablePaymentMethods = new List<PaymentMethodModel>();
            AvailableRoles = new List<CustomerRoleModel>();
            Restricted = new Dictionary<int, IDictionary<string, bool>>();
        }

        #endregion

        #region Properties

        public IList<PaymentMethodModel> AvailablePaymentMethods { get; set; }

        public IList<CustomerRoleModel> AvailableRoles { get; set; }

        //[Role id] / [shipping method id] / [restricted]
        public IDictionary<int, IDictionary<string, bool>> Restricted { get; set; }

        #endregion
    }
}