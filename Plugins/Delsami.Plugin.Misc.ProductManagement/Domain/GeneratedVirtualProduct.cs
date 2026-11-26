using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.ProductManagement.Domain
{
    public class GeneratedVirtualProduct : BaseEntity
    {
        public int MainParentProductId { get; set; }

        public int GeneratedParentProductId { get; set; }

        public int MainProductId { get; set; }

        public int GeneratedMainProductId { get; set; }

        public int? GeneratedVirtualProductId { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public bool Published { get; set; }
    }
}
