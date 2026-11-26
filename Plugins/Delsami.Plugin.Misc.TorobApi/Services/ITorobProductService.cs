using Delsami.Plugin.Misc.TorobApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.TorobApi.Services
{
    public interface ITorobProductService
    {
        Task<ProductResponse> GetAllProducts(int limit, int page);

        Task<ProductResponse> GetProductsByIds(List<int> productIds);

        Task<ProductResponse> GetProductsBySlugs(List<string> slugs);

    }
}
