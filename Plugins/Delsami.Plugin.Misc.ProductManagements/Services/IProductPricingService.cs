using ZarIran.Plugin.Misc.ProductManagements.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IProductPricingService
    {
        Task InsertAsync(ProductPricing zarIranProductPriceDetailsModel);
        Task UpdateAsync(ProductPricing zarIranProductPriceDetailsModel);
        Task<ProductPricing> GetByProductIdAsync(int productId);
    }
}
