using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Payments.Barook.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Delsami.Plugin.Payments.Barook.Configuration.TerminalCode")]
        public string TerminalCode { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Barook.Configuration.TerminalPassword")]
        public string TerminalPassword { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Barook.Configuration.BaseUrl")]
        public string BaseUrl { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Barook.Configuration.businessServiceSlug")]
        public string businessServiceSlug { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Barook.Configuration.paymentDailyCount")]
        public int paymentDailyCount { get; set; }


        public List<SelectListItem> AvaiablepaymentDailyCount { get; set; } = new();


    }
}
