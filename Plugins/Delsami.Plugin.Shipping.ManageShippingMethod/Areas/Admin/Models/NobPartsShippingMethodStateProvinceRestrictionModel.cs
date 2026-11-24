using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models;
using Nop.Web.Areas.Admin.Models.Shipping;
using System.Collections.Generic;
using ZarIran.Plugin.Misc.Core.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Models
{
    public record ZarIranShippingMethodStateProvinceRestrictionModel : BaseNopModel
    {
        #region Ctor

        public ZarIranShippingMethodStateProvinceRestrictionModel()
        {
            AvailableShippingMethods = new List<ShippingMethodModel>();
            AvailableStateProvinces = new List<SelectListItem>();
            Restricted = new Dictionary<int, IDictionary<int, bool>>();
            AvailableCities = new List<City>();
        }

        #endregion

        #region Properties

        public IList<ShippingMethodModel> AvailableShippingMethods { get; set; }

        public IList<SelectListItem> AvailableStateProvinces { get; set; }
        public IList<City> AvailableCities { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ManageShippingMethod.ProvinceId")]
        public int SelectedStateProviceId { get; set; }

        //[country id] / [shipping method id] / [restricted]
        public IDictionary<int, IDictionary<int, bool>> Restricted { get; set; }

        #endregion
    }
}