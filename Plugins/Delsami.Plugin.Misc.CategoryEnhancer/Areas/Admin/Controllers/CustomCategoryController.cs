using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Models;
using Delsami.Plugin.Misc.CategoryEnhancer.Domain;
using Delsami.Plugin.Misc.CategoryEnhancer.Services;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Events;
using Nop.Core.Infrastructure.Mapper;
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
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Events.Audit.Entities;
using Delsami.Plugin.Misc.Core.Extensions;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CustomCategoryController : BasePluginController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICategoryModelFactory _categoryModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICanonicalCategoryService _canonicalCategoryService;
        private readonly IDelsamiUrlRecordService _DelsamiUrlRecordService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategorySpecificationAttributeService _categorySpecificationAttributeService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public CustomCategoryController(IAclService aclService,
            ICategoryModelFactory categoryModelFactory,
            ICategorySpecificationAttributeService categorySpecificationAttributeService,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ISpecificationAttributeService specificationAttributeService,
            IUrlRecordService urlRecordService,
            ICanonicalCategoryService canonicalCategoryService,
            IDelsamiUrlRecordService DelsamiUrlRecordService,
            IEventPublisher eventPublisher)
        {
            _aclService = aclService;
            _categorySpecificationAttributeService = categorySpecificationAttributeService;
            _specificationAttributeService = specificationAttributeService;
            _categoryModelFactory = categoryModelFactory;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _canonicalCategoryService = canonicalCategoryService;
            _DelsamiUrlRecordService = DelsamiUrlRecordService;
            _eventPublisher = eventPublisher;
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

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await _categoryModelFactory.PrepareCategoryModelAsync(new CategoryModel(), null);

            var DelsamiModel = AutoMapperConfiguration.Mapper.Map<DelsamiCategoryModel>(model);
            DelsamiModel.AvailableSpecAttributes = await PrepareAvailableSpecAttributes();

            return View("~/Plugins/Delsami.Misc.CategoryEnhancer/Areas/Admin/Views/CustomCategory/Create.cshtml", DelsamiModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(DelsamiCategoryModel DelsamiModel, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var isValidCanonical = await _canonicalCategoryService.IsValidCanonicalAsync(DelsamiModel.CanonicalCategoryId);

            if (!isValidCanonical)
                ModelState.AddModelError("CanonicalCategoryId", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CategoryEnhancer.CategoryModel.InvalidCanonicalCategoryId"));

            var model = (CategoryModel)DelsamiModel;

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<Category>();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryService.InsertCategoryAsync(category);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(category, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                }

                await _categoryService.UpdateCategoryAsync(category);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(category);

                //ACL (customer roles)
                await SaveCategoryAclAsync(category, model);

                //stores
                await SaveStoreMappingsAsync(category, model);

                //add spec attribute
                if (DelsamiModel.SelectedSpecAttributesIds.Any())
                {
                    var selectedSpecs = DelsamiModel.SelectedSpecAttributesIds.Select(a => new CategorySpecificationAttribute()
                    {
                        CategoryId = category.Id,
                        SpecificationAttributeId = a
                    }).ToList();

                    await _categorySpecificationAttributeService.InsertRangeAsync(selectedSpecs);
                }


                if (DelsamiModel.CanonicalCategoryId != 0)
                    await _canonicalCategoryService.InsertAsync(new Domain.CanonicalCategory
                    {
                        CategoryId = category.Id,
                        CanonicalCategoryId = DelsamiModel.CanonicalCategoryId
                    });

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewCategory",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category.Name), category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Added"));

                if (!continueEditing)
                    return RedirectToAction("List", "Category");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = await _categoryModelFactory.PrepareCategoryModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Misc.CategoryEnhancer/Areas/Admin/Views/CustomCategory/Create.cshtml", DelsamiModel);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List", "Category");

            //prepare model
            var model = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);

            var DelsamiModel = AutoMapperConfiguration.Mapper.Map<DelsamiCategoryModel>(model);


            var canonicalCategory = await _canonicalCategoryService.GetByCategoryIdAsync(category.Id);

            if (canonicalCategory != null)
                DelsamiModel.CanonicalCategoryId = canonicalCategory.CanonicalCategoryId;

            var urlRecords = await _DelsamiUrlRecordService.GetSuggestionForCanonicalCategoriesAsync(category.Name, model.SeName, model.Id);

            var categories = await _categoryService.GetCategoriesByIdsAsync(urlRecords.Select(s => s.EntityId).ToArray());
            DelsamiModel.SuggestionCategories = urlRecords
                .Select(s => new
                {
                    Record = s,
                    Category = model.AvailableCategories
                        .FirstOrDefault(f => int.Parse(f.Value) == s.EntityId)
                })
                .Where(x => x.Category != null)
                .Select(x => new SuggestionCategoryModel
                {
                    SeName = x.Record.Slug,
                    Name = x.Category.Text,
                    Id = int.Parse(x.Category.Value)
                })
                .ToList();


            DelsamiModel.SelectedSpecAttributesIds = (await _categorySpecificationAttributeService.GetByCategoryId(category.Id)).Select(a => a.SpecificationAttributeId).ToList();
            DelsamiModel.AvailableSpecAttributes = await PrepareAvailableSpecAttributes();

            //DelsamiModel.SuggestionCategories = model.AvailableCategories
            //    .Where(w => urlRecords.Select(s => s.EntityId)
            //    .Contains(int.Parse(w.Value))).Select(s => new SuggestionCategoryModel
            //    {
            //        Id = int.Parse(s.Value),
            //        Name = s.Text,
            //        SeName =
            //    }).ToList();

            return View("~/Plugins/Delsami.Misc.CategoryEnhancer/Areas/Admin/Views/CustomCategory/Edit.cshtml", DelsamiModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(DelsamiCategoryModel DelsamiModel, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(DelsamiModel.Id);
            if (category == null || category.Deleted)
                return RedirectToAction("List", "Category");

            var oldcategory = category.MyDeepClone();

            var isValidCanonical = await _canonicalCategoryService.IsValidCanonicalAsync(DelsamiModel.CanonicalCategoryId);

            if (!isValidCanonical || DelsamiModel.CanonicalCategoryId == DelsamiModel.Id)
                ModelState.AddModelError("CanonicalCategoryId", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CategoryEnhancer.CategoryModel.InvalidCanonicalCategoryId"));

            var model = (CategoryModel)DelsamiModel;

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
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                            await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                    }
                    else
                    {
                        //remove discount
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

                //stores
                await SaveStoreMappingsAsync(category, model);

                //update spec attribute
                if (DelsamiModel.SelectedSpecAttributesIds.Any())
                {

                    await _categorySpecificationAttributeService.DeleteAllByCategoryId(category.Id);

                    var selectedSpecs = DelsamiModel.SelectedSpecAttributesIds.Select(a => new CategorySpecificationAttribute()
                    {
                        CategoryId = category.Id,
                        SpecificationAttributeId = a
                    }).ToList();
                    await _categorySpecificationAttributeService.InsertRangeAsync(selectedSpecs);
                }

                await _canonicalCategoryService.DeleteByCategoryIdAsync(category.Id);

                if (DelsamiModel.CanonicalCategoryId != 0)
                    await _canonicalCategoryService.InsertAsync(new Domain.CanonicalCategory
                    {
                        CategoryId = category.Id,
                        CanonicalCategoryId = DelsamiModel.CanonicalCategoryId
                    });


                //activity log
                await _customerActivityService.InsertActivityAsync("EditCategory",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCategory"), category.Name), category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

                await _eventPublisher.PublishAsync(new AuditUpdatedEntityEvent<Category>(category, oldcategory));

                if (!continueEditing)
                    return RedirectToAction("List", "Category");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = await _categoryModelFactory.PrepareCategoryModelAsync(DelsamiModel, category, true);

            var urlRecords = await _DelsamiUrlRecordService.GetSuggestionForCanonicalCategoriesAsync(category.Name, model.SeName, model.Id);

            var categories = await _categoryService.GetCategoriesByIdsAsync(urlRecords.Select(s => s.EntityId).ToArray());

            DelsamiModel.SuggestionCategories = urlRecords
              .Select(s => new
              {
                  Record = s,
                  Category = model.AvailableCategories
                      .FirstOrDefault(f => int.Parse(f.Value) == s.EntityId)
              })
              .Where(x => x.Category != null)
              .Select(x => new SuggestionCategoryModel
              {
                  SeName = x.Record.Slug,
                  Name = x.Category.Text,
                  Id = int.Parse(x.Category.Value)
              })
              .ToList();

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Delsami.Misc.CategoryEnhancer/Areas/Admin/Views/CustomCategory/Edit.cshtml", DelsamiModel);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return RedirectToAction("List");

            //Delete spec attribute
            await _categorySpecificationAttributeService.DeleteAllByCategoryId(id);

            await _categoryService.DeleteCategoryAsync(category);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

            return RedirectToAction("List", "Category");
        }

        //[HttpPost]
        //public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
        //        return AccessDeniedView();

        //    if (selectedIds == null || selectedIds.Count == 0)
        //        return NoContent();

        //    await _categoryService.DeleteCategoriesAsync(await (await _categoryService.GetCategoriesByIdsAsync(selectedIds.ToArray())).WhereAwait(async p => await _workContext.GetCurrentVendorAsync() == null).ToListAsync());

        //    return Json(new { Result = true });
        //}

        #endregion

        #region Utilities

        private async Task<List<SelectListItem>> PrepareAvailableSpecAttributes()
        {
            var specList = (await _specificationAttributeService.GetSpecificationAttributesAsync()).Where(a => a.SpecificationAttributeGroupId != null).ToList();
            return specList.Select(a => new SelectListItem()
            {
                Text = a.Name,
                Value = a.Id.ToString(),

            }).ToList();
        }
        #endregion
    }
}
