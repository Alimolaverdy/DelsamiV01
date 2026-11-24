using LinqToDB.Mapping;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Models
{
    public record BaseNopAjaxFiltersProModel : BaseNopModel
    {

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int TotalCount
        {
            get;
            set;
        }

        public BaseNopAjaxFiltersProModel()
        {
        }
    }
}
