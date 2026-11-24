using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record AvailableDeliveryTimeRangeModel : BaseNopEntityModel, ILocalizedModel<AvailableDeliveryTimeRangeLocalizedModel>
    {
        public AvailableDeliveryTimeRangeModel()
        {
            Locales = new List<AvailableDeliveryTimeRangeLocalizedModel>();

        }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.Time")]
        public string Time { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.DisplayOrder")]

        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.StartHour")]
        public TimeSpan StartHour { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.EndHour")]
        public TimeSpan EndHour { get; set; }

        public int StoreId { get; set; }

        public bool IsConfirmed { get; set; }

        public int? ExceptionDateId { get; set; }

        public IList<AvailableDeliveryTimeRangeLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.DaysOfWeek")]
        public IList<int> SelectedDayIds { get; set; }
        public string SelectedDayText { get; set; }
        public IList<SelectListItem> AvailableDays { get; set; } = new List<SelectListItem>();

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.ShippingMethods")]
        public IList<int> ShippingMethodIds { get; set; }
        public string ShippingMethodTexts { get; set; }
        public IList<SelectListItem> AvailableShippingMethod { get; set; } = new List<SelectListItem>();
    }

    public partial class AvailableDeliveryTimeRangeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.Time")]
        public string Time { get; set; }
    }
}
