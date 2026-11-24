using ZarIran.Plugin.Misc.CategoryEnhancer.Areas.Admin.Models;
using ZarIran.Plugin.Misc.CategoryEnhancer.Services;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Areas.Admin.Factroies
{
    public class ZarIranCategoryModelFactory : CategoryModelFactory
    {
        #region Fields

        private readonly ICategoryIconService _categoryIconService;

        #endregion

        #region Ctor

        public ZarIranCategoryModelFactory(CatalogSettings catalogSettings,
                                            CurrencySettings currencySettings,
                                            ICurrencyService currencyService,
                                            IAclSupportedModelFactory aclSupportedModelFactory,
                                            IBaseAdminModelFactory baseAdminModelFactory,
                                            ICategoryService categoryService,
                                            IDiscountService discountService,
                                            IDiscountSupportedModelFactory discountSupportedModelFactory,
                                            ILocalizationService localizationService,
                                            ILocalizedModelFactory localizedModelFactory,
                                            IProductService productService,
                                            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
                                            IUrlRecordService urlRecordService,
                                            ICategoryIconService categoryIconService) : base(catalogSettings, currencySettings, currencyService, aclSupportedModelFactory, baseAdminModelFactory, categoryService, discountService, discountSupportedModelFactory, localizationService, localizedModelFactory, productService, storeMappingSupportedModelFactory, urlRecordService)
        {
            _categoryIconService = categoryIconService;
        }

        #endregion

        #region Methods

        public override async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false)
        {
            model = await base.PrepareCategoryModelAsync(model, category, excludeProperties);

            if (model is ZarIranCategoryModel extended)
            {
                if (category == null)
                {
                    extended.IconId = 0;
                }
                else
                {
                    var icon = await _categoryIconService.GetByCategoryIdAsync(category.Id);
                    extended.IconId = icon?.IconId ?? 0;
                }
            }

            return model;
        }

        #endregion
    }
}