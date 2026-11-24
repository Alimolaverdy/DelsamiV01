using Nop.Web.Framework.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Enums;
using System.Collections.Generic;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Models
{
    public record NopAjaxFiltersProPublicModel : BaseNopModel
    {

        public IList<BaseNopAjaxFiltersProModel> Specifications
        {
            get;
            set;
        }

        public NopAjaxFiltersProUIMode UIMode
        {
            get;
            set;
        }

        public NopAjaxFiltersProPublicModel()
        {
            this.Specifications = new List<BaseNopAjaxFiltersProModel>();
        }
    }
}
