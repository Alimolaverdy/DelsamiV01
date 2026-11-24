using Nop.Core;
using System;

namespace ZarIran.Plugin.Misc.ProductManagements.Domain
{
    public class ZarIranBackInStockSubscription: BaseEntity
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string MobileNumber { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool IsNotified { get; set; }
        public bool IsActive { get; set; }
    }
}
