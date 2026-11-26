using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.BundleProducts;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Products;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.VirtualProducts;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Factories
{
    public interface IDelsamiProductModelFactory
    {
        Task<DelsamiProductListModel> PrepareDelsamiProductListModelAsync(ProductSearchModel searchModel);

        Task<AddVirtualProductSearchModel> PrepareAddVirtualProductSearchModelAsync(AddVirtualProductSearchModel searchModel);

        Task<VirtualProductListModel> PrepareVirtualProductListModelAsync(VirtualProductSearchModel searchModel, Product product);

        Task<AddVirtualProductListModel> PrepareAddVirtualProductListModelAsync(AddVirtualProductSearchModel searchModel);

        Task<DelsamiProductModel> PrepareDelsamiProductModelAsync(DelsamiProductModel model, Product product, bool excludeProperties = false);

        Task<DelsamiProductSearchModel> PrepareProductSearchModelAsync(DelsamiProductSearchModel searchModel);

        Task<ProductListModel> PrepareProductListModelAsync(DelsamiProductSearchModel searchModel);

        Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int productId, int? specificationId,int specAttribId, bool isSelected = true);

        Task<BundleProductListModel> PrepareBundleProductListModelAsync(BundleProductSearchModel searchModel, Product product);

        Task<AddBundleProductSearchModel> PrepareAddBundleProductSearchModelAsync(AddBundleProductSearchModel searchModel);

        Task<AddBundleProductListModel> PrepareAddBundleProductListModelAsync(AddBundleProductSearchModel searchModel);

    }
}
