using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using SevenSpikes.Nop.Framework.Controllers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain.Enums;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.ManufacturerFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.SpecificationFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Settings;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Extensions;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.QueryStringManipulation;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Catalog.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable enable
namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Controllers
{
    public class AjaxFiltersProSearchController : Base7SpikesPublicController
    {

        private readonly ICategoryService _categoryService;
        private readonly ICategoryService7Spikes _categoryService7Spikes;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductServiceNopAjaxFilters _productServiceNopAjaxFilters;
        private readonly IQueryStringBuilder _queryStringBuilder;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeService7Spikes _specificationAttributeService7Spikes;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;
        private readonly NopAjaxFiltersSettings _nopAjaxFiltersSettings;
        private readonly NopAjaxFiltersProSettings _nopAjaxFiltersProSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IGenericAttributeService _genericAttributeService;

        public AjaxFiltersProSearchController(
          ICategoryService categoryService,
          ICategoryService7Spikes categoryService7Spikes,
          ILocalizationService localizationService,
          IManufacturerService manufacturerService,
          IProductServiceNopAjaxFilters productServiceNopAjaxFilters,
          IQueryStringBuilder queryStringBuilder,
          ISpecificationAttributeService specificationAttributeService,
          ISpecificationAttributeService7Spikes specificationAttributeService7Spikes,
          IStoreContext storeContext,
          IStaticCacheManager staticCacheManager,
          IWorkContext workContext,
          NopAjaxFiltersSettings nopAjaxFiltersSettings,
          NopAjaxFiltersProSettings nopAjaxFiltersProSettings,
          CatalogSettings catalogSettings,
          LocalizationSettings localizationSettings,
          IGenericAttributeService genericAttributeService)
        {
            this._categoryService = categoryService;
            this._categoryService7Spikes = categoryService7Spikes;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._productServiceNopAjaxFilters = productServiceNopAjaxFilters;
            this._queryStringBuilder = queryStringBuilder;
            this._specificationAttributeService = specificationAttributeService;
            this._specificationAttributeService7Spikes = specificationAttributeService7Spikes;
            this._storeContext = storeContext;
            this._staticCacheManager = staticCacheManager;
            this._workContext = workContext;
            this._nopAjaxFiltersSettings = nopAjaxFiltersSettings;
            this._nopAjaxFiltersProSettings = nopAjaxFiltersProSettings;
            this._catalogSettings = catalogSettings;
            this._localizationSettings = localizationSettings;
            this._genericAttributeService = genericAttributeService;
        }

        [HttpGet]
        public ActionResult GetCategories() => (ActionResult)new EmptyResult();

        [HttpPost]
        public async Task<ActionResult> GetCategories(int parentCategoryId = 0)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);
            var workingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id;
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                CacheKeys.AjaxFiltersProCategoriesCacheKey,
                customerRoleIds,
                workingLanguageId,
                storeId,
                parentCategoryId);

            var categories = await _staticCacheManager.GetAsync<IList<BaseNopAjaxFiltersProModel>>(
                cacheKey,
                async () =>
                {
                    var excludedCategoryIds = _nopAjaxFiltersSettings.CategoriesWithoutFilters
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => id.Trim())
                        .ToHashSet();

                    var result = new List<BaseNopAjaxFiltersProModel>();
                    if (_nopAjaxFiltersProSettings.ShowTopLevelCategoriesOnly || parentCategoryId > 0)
                    {
                        var source = await _categoryService7Spikes
                            .GetCategoriesByParentCategoryIdAsync(parentCategoryId, false, false);

                        foreach (var category in source
                                     .Where(category => !excludedCategoryIds.Contains(category.Id.ToString()))
                                     .OrderBy(category => category.ParentCategoryId)
                                     .ThenBy(category => category.DisplayOrder))
                        {
                            result.Add(new BaseNopAjaxFiltersProModel
                            {
                                Id = category.Id,
                                Name = await _localizationService.GetLocalizedAsync(
                                    category,
                                    item => item.Name,
                                    (int?)null,
                                    true,
                                    true)
                            });
                        }
                    }
                    else
                    {
                        var source = await _categoryService.GetAllCategoriesAsync();
                        foreach (var category in source.Where(category =>
                                     !excludedCategoryIds.Contains(category.Id.ToString())))
                        {
                            result.Add(new BaseNopAjaxFiltersProModel
                            {
                                Id = category.Id,
                                Name = await _categoryService.GetFormattedBreadCrumbAsync(
                                    category,
                                    languageId: workingLanguageId)
                            });
                        }
                    }

                    if (parentCategoryId > 0)
                    {
                        result.Insert(0, new BaseNopAjaxFiltersProModel
                        {
                            Id = 0,
                            Name = await _localizationService.GetResourceAsync(
                                "SevenSpikes.NopAjaxFiltersPro.Client.AllAvailable"),
                            TotalCount = result.Count
                        });
                    }

                    return result;
                });

            return Json(categories);
        }

        [HttpGet]
        public ActionResult GetManufacturers() => (ActionResult)new EmptyResult();

        [HttpPost]
        public async Task<ActionResult> GetManufacturers(int categoryId = 0)
        {
            List<int> categoryIds1 = new List<int>();
            if (categoryId > 0)
            {
                Category categoryByIdAsync = await this._categoryService.GetCategoryByIdAsync(categoryId);
                categoryIds1.Add(categoryByIdAsync.Id);
                if (this._catalogSettings.ShowProductsFromSubcategories)
                    categoryIds1.AddRange((IEnumerable<int>)await this._categoryService7Spikes.GetCategoryIdsByParentCategoryAsync(categoryByIdAsync.Id));
            }
            IProductServiceNopAjaxFilters serviceNopAjaxFilters = this._productServiceNopAjaxFilters;
            IList<int> categoryIds2 = (IList<int>)categoryIds1;
            ProductsResultDataDTO productsResultDataDto1 = await serviceNopAjaxFilters.SearchProductsAsync(categoryIds2, (SpecificationFilterModelDTO)null, (AttributeFilterModelDTO)null, (ManufacturerFilterModelDTO)null, (VendorFilterModelDTO)null, pageSize: 1, storeId: (await this._storeContext.GetCurrentStoreAsync()).Id);
            serviceNopAjaxFilters = (IProductServiceNopAjaxFilters)null;
            categoryIds2 = (IList<int>)null;
            ProductsResultDataDTO productsResultDataDto = productsResultDataDto1;
            IList<Manufacturer> manufacturers = (IList<Manufacturer>)new List<Manufacturer>();
            if (productsResultDataDto.ManufacturerIds != null && productsResultDataDto.ManufacturerIds.Any<int>())
                manufacturers = this._manufacturerService.GetManufacturersByIds(productsResultDataDto.ManufacturerIds.ToArray<int>());
            IList<BaseNopAjaxFiltersProModel> models = (IList<BaseNopAjaxFiltersProModel>)new List<BaseNopAjaxFiltersProModel>();
            IList<BaseNopAjaxFiltersProModel> ajaxFiltersProModelList;
            BaseNopAjaxFiltersProModel ajaxFiltersProModel1;
            BaseNopAjaxFiltersProModel ajaxFiltersProModel2;
            if (categoryId != 0)
            {
                ajaxFiltersProModelList = models;
                ajaxFiltersProModel2 = new BaseNopAjaxFiltersProModel();
                ajaxFiltersProModel2.Id = 0;
                ajaxFiltersProModel1 = ajaxFiltersProModel2;
                ajaxFiltersProModel1.Name = await this._localizationService.GetResourceAsync("SevenSpikes.NopAjaxFiltersPro.Client.AllAvailable");
                ajaxFiltersProModel2.TotalCount = productsResultDataDto.ProductsPagedList.TotalCount;
                ajaxFiltersProModelList.Add(ajaxFiltersProModel2);
                ajaxFiltersProModelList = (IList<BaseNopAjaxFiltersProModel>)null;
                ajaxFiltersProModel1 = (BaseNopAjaxFiltersProModel)null;
                ajaxFiltersProModel2 = (BaseNopAjaxFiltersProModel)null;
            }
            foreach (Manufacturer manufacturer in (IEnumerable<Manufacturer>)manufacturers)
            {
                ajaxFiltersProModelList = models;
                ajaxFiltersProModel1 = new BaseNopAjaxFiltersProModel();
                ajaxFiltersProModel1.Id = manufacturer.Id;
                ajaxFiltersProModel2 = ajaxFiltersProModel1;
                ILocalizationService localizationService = this._localizationService;
                Manufacturer entity = manufacturer;
                Expression<Func<Manufacturer, string>> keySelector = (Expression<Func<Manufacturer, string>>)(x => x.Name);
                int? languageId = new int?();
                ajaxFiltersProModel2.Name = await localizationService.GetLocalizedAsync<Manufacturer, string>(entity, keySelector, languageId);
                ajaxFiltersProModelList.Add(ajaxFiltersProModel1);
                ajaxFiltersProModelList = (IList<BaseNopAjaxFiltersProModel>)null;
                ajaxFiltersProModel2 = (BaseNopAjaxFiltersProModel)null;
                ajaxFiltersProModel1 = (BaseNopAjaxFiltersProModel)null;
            }
            if (categoryId == 0 && models.Count > 0)
                models[0].TotalCount = productsResultDataDto.ProductsPagedList.TotalCount;
            ActionResult manufacturers1 = (ActionResult)this.Json(models);
            categoryIds1 = (List<int>)null;
            productsResultDataDto = (ProductsResultDataDTO)null;
            manufacturers = (IList<Manufacturer>)null;
            models = (IList<BaseNopAjaxFiltersProModel>)null;
            return manufacturers1;
        }

        [HttpGet]
        public ActionResult GetSpecification() => new EmptyResult();

        [HttpPost]
        public async Task<ActionResult> GetSpecification(
          int specificationId = 0,
          int categoryId = 0,
          int manufacturerId = 0,
          IList<int> selectedSpecificationIds = null)
        {
            IList<int> intList = new List<int>();
            if (selectedSpecificationIds != null && selectedSpecificationIds.Count > 0)
            {
                foreach (int selectedSpecificationId in (IEnumerable<int>)selectedSpecificationIds)
                {
                    if (selectedSpecificationId != 0)
                        intList.Add(selectedSpecificationId);
                }
            }

            var specificationFilterModelDto = new SpecificationFilterModelDTO();

            if (intList.Count > 0)
                specificationFilterModelDto.SpecificationFilterDTOs.Add(new SpecificationFilterDTO()
                {
                    Id = specificationId,
                    SelectedFilterIds = intList
                });

            ManufacturerFilterModelDTO manufacturerFilterModelDto = new();

            if (manufacturerId > 0 && categoryId > 0)
                manufacturerFilterModelDto.SelectedFilterIds.Add(manufacturerId);

            List<int> categoryIds1 = new();

            if (categoryId > 0)
            {
                Category categoryByIdAsync = await _categoryService.GetCategoryByIdAsync(categoryId);
                categoryIds1.Add(categoryByIdAsync.Id);
                if (_catalogSettings.ShowProductsFromSubcategories)
                    categoryIds1.AddRange((IEnumerable<int>)await _categoryService7Spikes.GetCategoryIdsByParentCategoryAsync(categoryByIdAsync.Id));
            }

            int num = 0;
            if (manufacturerId > 0 && categoryId == 0)
                num = manufacturerId;

            IProductServiceNopAjaxFilters serviceNopAjaxFilters = _productServiceNopAjaxFilters;
            IList<int> categoryIds2 = categoryIds1;
            SpecificationFilterModelDTO specifiationFilterModelDTO = specificationFilterModelDto;
            ManufacturerFilterModelDTO manufacturerFilterModelDTO = manufacturerFilterModelDto;
            int manufacturerId1 = num;
            ProductsResultDataDTO productsResultDataDto1 = await serviceNopAjaxFilters.SearchProductsAsync(categoryIds2, specifiationFilterModelDTO, (AttributeFilterModelDTO)null, manufacturerFilterModelDTO, (VendorFilterModelDTO)null, pageSize: 1, manufacturerId: manufacturerId1, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            serviceNopAjaxFilters = null;
            categoryIds2 = null;
            specifiationFilterModelDTO = null;
            manufacturerFilterModelDTO = null;
            ProductsResultDataDTO productsResultDataDto = productsResultDataDto1;

            List<BaseNopAjaxFiltersProModel> models = new();
            IList<BaseNopAjaxFiltersProModel> ajaxFiltersProModelList = models;
            BaseNopAjaxFiltersProModel ajaxFiltersProModel1 = new();
            ajaxFiltersProModel1.Id = 0;
            BaseNopAjaxFiltersProModel ajaxFiltersProModel2 = ajaxFiltersProModel1;
            ajaxFiltersProModel2.Name = await _localizationService.GetResourceAsync("SevenSpikes.NopAjaxFiltersPro.Client.AllAvailable");
            ajaxFiltersProModel1.TotalCount = productsResultDataDto.ProductsPagedList.TotalCount;
            ajaxFiltersProModelList.Add(ajaxFiltersProModel1);
            ajaxFiltersProModelList = null;
            ajaxFiltersProModel2 = null;
            ajaxFiltersProModel1 = null;

            if (specificationId == 0)
                return Json(models);

            var specificationAttributeOptions = await _specificationAttributeService7Spikes.GetSpecificationAttributeOptionsByIdsAndSpecificationIdAsync(productsResultDataDto.SpecificationOptionIds.ToArray(), specificationId);

            if (specificationAttributeOptions != null && specificationAttributeOptions.Count > 0)
            {
                foreach (var specificationAttributeOption in specificationAttributeOptions.OrderBy(x => x.DisplayOrder).ToList())
                {
                    ajaxFiltersProModelList = models;
                    ajaxFiltersProModel2 = new BaseNopAjaxFiltersProModel();
                    ajaxFiltersProModel2.Id = specificationAttributeOption.Id;
                    ajaxFiltersProModel1 = ajaxFiltersProModel2;
                    ILocalizationService localizationService = _localizationService;
                    SpecificationAttributeOption entity = specificationAttributeOption;
                    int? languageId = new int?();
                    ajaxFiltersProModel1.Name = await localizationService.GetLocalizedAsync(entity, x => x.Name, languageId);
                    ajaxFiltersProModelList.Add(ajaxFiltersProModel2);
                    ajaxFiltersProModelList = null;
                    ajaxFiltersProModel1 = null;
                    ajaxFiltersProModel2 = null;
                }
            }
            return Json(models);
        }

        [HttpGet]
        public ActionResult Search() => new EmptyResult();

        [HttpPost]
        public async Task<ActionResult> Search(
          int categoryId = 0,
          int subCategoryId = 0,
          int manufacturerId = 0,
          params int[] specificationOptions)
        {
            if (subCategoryId > 0)
                categoryId = subCategoryId;

            if (categoryId == 0 && manufacturerId == 0)
                return RedirectToRoute("HomePage");

            SpecificationFilterModel7Spikes specificationFilterModel7Spikes = new();

            bool hasOneOption = false;
            if (specificationOptions != null)
            {
                int[] numArray = specificationOptions;
                for (int index = 0; index < numArray.Length; ++index)
                {
                    int specificationOption = numArray[index];
                    if (specificationOption != 0)
                    {
                        hasOneOption = true;
                        await PrepareSpecificationFilterModelAsync(specificationOption, specificationFilterModel7Spikes);
                    }
                }
            }

            ManufacturerFilterModel7Spikes manufacturerFilterModel7Spikes = new();

            if (manufacturerId > 0 && categoryId > 0)
                PrepareManufacturerFilterModel(manufacturerId, manufacturerFilterModel7Spikes);

            _queryStringBuilder.SetDataForQueryString(specificationFilterModel7Spikes, null, manufacturerFilterModel7Spikes, null, null, null, null, null);

            IUrlHelper helper;
            string str;

            if (manufacturerId > 0 && categoryId == 0)
            {
                Manufacturer manufacturerByIdAsync = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);
                helper = Url;
                str = helper.RouteUrl("Manufacturer", new
                {
                    SeName = await UrlRecordService.GetSeNameAsync(manufacturerByIdAsync)
                });
            }
            else
            {
                Category categoryByIdAsync = await _categoryService.GetCategoryByIdAsync(categoryId);
                helper = Url;
                str = helper.RouteUrl("Category", new
                {
                    SeName = await UrlRecordService.GetSeNameAsync(categoryByIdAsync)
                });
            }

            string hashQuery = _queryStringBuilder.GetQueryString(true);
            string url1 = str;
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = (string)HttpContext.Request.PathBase;
                if (!(await url1.IsLocalizedUrlAsync((PathString)applicationPath, true)).Item1)
                {
                    PathString remaining;
                    new PathString(url1).StartsWithSegments((PathString)applicationPath, out remaining);
                    string url2 = remaining.Value;
                    PathString pathBase = (PathString)applicationPath;
                    url1 = url2.AddLanguageSeoCodeToUrl(pathBase, false, await _workContext.GetWorkingLanguageAsync());
                    url2 = (string)null;
                    pathBase = new PathString();
                    url1 = applicationPath + url1;
                }
            }
            url1 = url1 + "?fromHomePageFilter=true";

            if (((manufacturerId <= 0 ? 0 : (categoryId > 0 ? 1 : 0)) | (hasOneOption ? 1 : 0)) != 0)
                url1 = url1 + "#/" + hashQuery;

            return LocalRedirect(url1);
        }

        private void PrepareManufacturerFilterModel(
          int manufacturerId,
          ManufacturerFilterModel7Spikes manufacturerFilterModel7Spikes)
        {
            ManufacturerFilterItem manufacturerFilterItem = new ManufacturerFilterItem()
            {
                Id = manufacturerId,
                FilterItemState = FilterItemState.Checked
            };
            manufacturerFilterModel7Spikes.ManufacturerFilterItems.Add(manufacturerFilterItem);
        }

        private async Task PrepareSpecificationFilterModelAsync(int specificationOption,
                                                                SpecificationFilterModel7Spikes specificationFilterModel7Spikes)
        {
            var attributeOptionByIdAsync = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(specificationOption);
            var specificationFilterItem = new SpecificationFilterItem()
            {
                Id = attributeOptionByIdAsync.Id,
                FilterItemState = FilterItemState.Checked
            };

            specificationFilterModel7Spikes.SpecificationFilterGroups.Add(new SpecificationFilterGroup()
            {
                Id = attributeOptionByIdAsync.SpecificationAttributeId,
                FilterItems = new List<SpecificationFilterItem>() { specificationFilterItem }
            });
        }

    }
}
