using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.CategoryEnhancer.Areas.Admin.Models;
using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using ZarIran.Plugin.Misc.CategoryEnhancer.Services;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core.Caching;
using ZarIran.Plugin.Misc.Core.Extensions;
using System.Collections.Generic;
using Nop.Core;


namespace ZarIran.Plugin.Misc.CategoryEnhancer.Areas.Admin.Controllers
{
    public class ZarIranCategoryEnhancerController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ICategoryModelFactory _categoryModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryIconService _categoryIconService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly IAclService _aclService;
        private readonly ICustomerService _customerService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ZarIranCategoryEnhancerController(IPermissionService permissionService,
                                                  ICategoryModelFactory categoryModelFactory,
                                                  ICategoryService categoryService,
                                                  IUrlRecordService urlRecordService,
                                                  IDiscountService discountService,
                                                  ICustomerActivityService customerActivityService,
                                                  INotificationService notificationService,
                                                  ILocalizationService localizationService,
                                                  ICategoryIconService categoryIconService,
                                                  ILocalizedEntityService localizedEntityService,
                                                  IPictureService pictureService,
                                                  IAclService aclService,
                                                  ICustomerService customerService,
                                                  IStoreMappingService storeMappingService,
                                                  IStoreService storeService,
                                                  IStaticCacheManager staticCacheManager,
                                                  IWorkContext workContext)
        {
            _permissionService = permissionService;
            _categoryModelFactory = categoryModelFactory;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _discountService = discountService;
            _customerActivityService = customerActivityService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _categoryIconService = categoryIconService;
            _localizedEntityService = localizedEntityService;
            _pictureService = pictureService;
            _aclService = aclService;
            _customerService = customerService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> Create()
        {
            var model = await _categoryModelFactory.PrepareCategoryModelAsync(new ZarIranCategoryModel(), null);

            return View("~/Plugins/ZarIran.Misc.CategoryEnhancer/Areas/Admin/Views/CategoryEnhancer/Create.cshtml", model);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ZarIranCategoryModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<Category>();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;

                await _categoryService.InsertCategoryAsync(category);

                // SEO
                model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

                // Locales
                await UpdateLocalesAsync(category, model);

                // Discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        await _categoryService.InsertDiscountCategoryMappingAsync(
                            new DiscountCategoryMapping
                            {
                                DiscountId = discount.Id,
                                EntityId = category.Id
                            });
                    }
                }

                await _categoryService.UpdateCategoryAsync(category);

                // Picture SEO
                await UpdatePictureSeoNamesAsync(category);

                // ACL
                await SaveCategoryAclAsync(category, model);

                // Stores
                await SaveStoreMappingsAsync(category, model);

                await _categoryIconService.InsertAsync(new CategoryIcon
                {
                    CategoryId = category.Id,
                    IconId = model.IconId
                });

                // Log
                await _customerActivityService.InsertActivityAsync("AddNewCategory",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category.Name), category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Added"));

                if (!continueEditing)
                    return RedirectToAction("List", "Category", new { area = "Admin" });

                return RedirectToAction("Edit", new { id = category.Id });
            }

            var baseModel = await _categoryModelFactory.PrepareCategoryModelAsync(model, null, true);

            var extendedModel = new ZarIranCategoryModel();

            baseModel.CopyPropertiesTo(extendedModel);

            extendedModel.IconId = model.IconId;

            return View("~/Plugins/ZarIran.Plugin.Misc.CategoryEnhancer/Areas/Admin/Views/CategoryEnhancer/Create.cshtml", extendedModel);
        }



        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List", "Category", new { area = "Admin" });

            //prepare base model
            var baseModel = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);

            var extendedModel = new ZarIranCategoryModel();

            baseModel.CopyPropertiesTo(extendedModel);

            var categoryIcon = await _categoryIconService.GetByCategoryIdAsync(category.Id);
            extendedModel.IconId = categoryIcon?.IconId ?? 0;

            return View("~/Plugins/ZarIran.Misc.CategoryEnhancer/Areas/Admin/Views/CategoryEnhancer/Edit.cshtml", extendedModel);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ZarIranCategoryModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(model.Id);
            if (category == null || category.Deleted)
                return RedirectToAction("List", "Category", new { area = "Admin" });

            if (ModelState.IsValid)
            {
                var prevPictureId = category.PictureId;

                //if parent category changes, we need to clear cache for previous parent category
                if (category.ParentCategoryId != model.ParentCategoryId)
                {
                    await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesByParentCategoryPrefix, category.ParentCategoryId);
                    await _staticCacheManager.RemoveByPrefixAsync(NopCatalogDefaults.CategoriesChildIdsPrefix, category.ParentCategoryId);
                }

                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryService.UpdateCategoryAsync(category);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(category, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                            await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                    }
                    else
                    {
                        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                            await _categoryService.DeleteDiscountCategoryMappingAsync(mapping);
                    }
                }

                await _categoryService.UpdateCategoryAsync(category);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(category);

                //ACL
                await SaveCategoryAclAsync(category, model);

                //Stores
                await SaveStoreMappingsAsync(category, model);

                //category icon Insert Or Update
                await _categoryIconService.InsertAsync(new CategoryIcon
                {
                    CategoryId = category.Id,
                    IconId = model.IconId
                });


                //activity log
                await _customerActivityService.InsertActivityAsync("EditCategory",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCategory"), category.Name), category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List", "Category", new { area = "Admin" });

                return RedirectToAction("Edit", new { id = category.Id });
            }

            var baseModel = await _categoryModelFactory.PrepareCategoryModelAsync(model, category, true);

            var extendedModel = new ZarIranCategoryModel();

            baseModel.CopyPropertiesTo(extendedModel);

            extendedModel.IconId = model.IconId;

            return View("~/Plugins/ZarIran.Misc.CategoryEnhancer/Areas/Admin/Views/CategoryEnhancer/Edit.cshtml", extendedModel);
        }


        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return RedirectToAction("List", "Category", new { area = "Admin" });

            await _categoryService.DeleteCategoryAsync(category);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

            return RedirectToAction("List", "Category", new { area = "Admin" });
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Category category, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(category, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(category, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Category category)
        {
            var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(category.Name));
        }

        protected virtual async Task SaveCategoryAclAsync(Category category, CategoryModel model)
        {
            category.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await _categoryService.UpdateCategoryAsync(category);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(category);
            var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.CustomerRoleId == customerRole.Id))
                        await _aclService.InsertAclRecordAsync(category, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Category category, CategoryModel model)
        {
            category.LimitedToStores = model.SelectedStoreIds.Any();
            await _categoryService.UpdateCategoryAsync(category);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(category);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(category, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion
    }
}
