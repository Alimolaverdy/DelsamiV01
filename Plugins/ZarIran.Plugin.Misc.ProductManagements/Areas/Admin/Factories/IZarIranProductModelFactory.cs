using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.Products;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.VirtualProducts;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Factories
{
    public interface IZarIranProductModelFactory
    {
        Task<ZarIranProductListModel> PrepareZarIranProductListModelAsync(ProductSearchModel searchModel);

        Task<AddVirtualProductSearchModel> PrepareAddVirtualProductSearchModelAsync(AddVirtualProductSearchModel searchModel);

        Task<VirtualProductListModel> PrepareVirtualProductListModelAsync(VirtualProductSearchModel searchModel, Product product);

        Task<AddVirtualProductListModel> PrepareAddVirtualProductListModelAsync(AddVirtualProductSearchModel searchModel);

        Task<ZarIranProductModel> PrepareZarIranProductModelAsync(ZarIranProductModel model, Product product, bool excludeProperties = false);

        Task<ZarIranProductSearchModel> PrepareProductSearchModelAsync(ZarIranProductSearchModel searchModel);

        Task<ProductListModel> PrepareProductListModelAsync(ZarIranProductSearchModel searchModel);

        Task<OtherBrandProductListModel> PrepareOtherBrandProductListModelAsync(OtherBrandProductSearchModel searchModel, Product product);

        Task<AddOtherBrandProductSearchModel> PrepareAddOtherBrandProductSearchModelAsync(AddOtherBrandProductSearchModel searchModel);

        Task<AddOtherBrandProductListModel> PrepareAddOtherBrandProductListModelAsync(AddOtherBrandProductSearchModel searchModel);


        Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int productId, int? specificationId,int specAttribId, bool isSelected = true);

        Task<BundleProductListModel> PrepareBundleProductListModelAsync(BundleProductSearchModel searchModel, Product product);

        Task<AddBundleProductSearchModel> PrepareAddBundleProductSearchModelAsync(AddBundleProductSearchModel searchModel);

        Task<AddBundleProductListModel> PrepareAddBundleProductListModelAsync(AddBundleProductSearchModel searchModel);

    }
}
