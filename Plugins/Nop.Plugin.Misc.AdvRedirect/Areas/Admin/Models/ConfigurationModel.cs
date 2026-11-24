using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableTypes = new List<SelectListItem>();
            SearchModel = new RedirectionSearchModel();
        }

        public RedirectionSearchModel SearchModel { get; set; }

        public RedirectionModel AddRedirection { get; set; }

        public IList<SelectListItem> AvailableTypes { get; set; }

        public string Pattern { get; set; }

        public string RedirectUrl { get; set; }

        public bool EnableAdvRedirectHistoryLogging { get; set; }

    }
}