using ZarIran.Plugin.Misc.ProductManagements.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IOtherBrandProductService
    {
        Task<IList<OtherBrandProduct>> GetOtherBrandProductsByProductId1Async(int productId1, bool showHidden = false);

        Task<OtherBrandProduct> GetOtherBrandProductByIdAsync(int otherBrandProductId);

        Task UpdateOtherBrandProductAsync(OtherBrandProduct otherBrandProduct);

        Task DeleteOtherBrandProductAsync(OtherBrandProduct otherBrandProduct);

        OtherBrandProduct FindOtherBrandProduct (IList<OtherBrandProduct> source, int productId1, int productId2);

        Task InsertOtherBrandProductAsync(OtherBrandProduct otherBrandProduct);
    }
}
