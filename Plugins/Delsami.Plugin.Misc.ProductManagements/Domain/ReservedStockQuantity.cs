using Nop.Core;
using System;

namespace ZarIran.Plugin.Misc.ProductManagements.Domain
{
    public class ReservedStockQuantity : BaseEntity
    {
        public int ProductId { get; set; }

        public int Count { get; set; }

        public DateTime LastModifiedDateTime { get; set; }

    }
}
