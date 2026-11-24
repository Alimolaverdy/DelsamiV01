using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ProductManagements.Domain
{
    public class ProductPricing : BaseEntity
    {
        public int ProductId { get; set; }
        public decimal OldMakingFee { get; set; }
        public decimal MakingFee { get; set; }
        public decimal OldProfitPercent { get; set; }
        public decimal ProfitPercent { get; set; }
        public decimal OldCoWorkerProfitPercent { get; set; }
        public decimal CoWorkerProfitPercent { get; set; }
        public decimal CoWorkerMakingFee { get; set; }
        public decimal OldCoWorkerMakingFee { get; set; }
        public decimal AdditionalAmount { get; set; }
    }
}
