using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.Core.Domain;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models;

public record ZarIranPaymentMethodRestrictionModel : BaseNopModel
{
    #region Ctor

    public ZarIranPaymentMethodRestrictionModel()
    {
        AvailablePaymentMethods = new List<PaymentMethodModel>();
        AvailableStateProvinces = new List<SelectListItem>();
        Restricted = new Dictionary<int, IDictionary<string, bool>>();
        AvailableCities = new List<City>();
    }

    #endregion

    #region Properties

    public IList<PaymentMethodModel> AvailablePaymentMethods { get; set; }

    public IList<SelectListItem> AvailableStateProvinces { get; set; }
    public IList<City> AvailableCities { get; set; }

    [NopResourceDisplayName("ZarIran.Plugin.Payment.ManagePaymentMethod.SelectedStateProviceId")]
    public int SelectedStateProviceId { get; set; }

    //[country id] / [shipping method id] / [restricted]
    public IDictionary<int, IDictionary<string, bool>> Restricted { get; set; }

    #endregion
}