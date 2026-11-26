using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Delsami.Plugin.Misc.CategoryEnhancer.Services;
using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Misc.Core.Domain;
using Delsami.Plugin.Misc.Core.Enums;
using Delsami.Plugin.Misc.Core.Events.Audit.Entities;
using Delsami.Plugin.Misc.Core.Extensions;
using Delsami.Plugin.Misc.Core.Services;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Factories;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.BundleProducts;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Products;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.VirtualProducts;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Delsami.Plugin.Misc.ProductManagement.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Enums;
using Nop.Plugin.Misc.AdvRedirect.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Controllers
{
	public class CustomProductController : ProductController
	{
		#region Fields

		private readonly IWorkContext _workContext;
		private readonly ILocalizedEntityService _localizedEntityService;
		private readonly ICustomerService _customerService;
		private readonly IImportReportService _importReportService;
		private readonly ISpecificationAttributeService _specificationService;
		private readonly IRedirectionsService _redirectionsService;
		private readonly ICustomDelsamiProductService _customProductService;
		private readonly IExportManager _exportManager;
		private readonly ICategoryService _categoryService;
		private readonly VendorSettings _vendorSettings;
		private readonly ProductManagementSettings _productManagementSettings;
		private readonly CoreSettings _coreSettings;
		private readonly IProductService _productService;
		private readonly INotificationService _notificationService;
		private readonly ILocalizationService _localizationService;
		private readonly IPermissionService _permissionService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly IUrlRecordService _urlRecordService;
		private readonly IProductTagService _productTagService;
		private readonly ICustomerActivityService _customerActivityService;
		private readonly IStoreContext _storeContext;
		private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
		private readonly IDownloadService _downloadService;
		private readonly IShippingService _shippingService;
		private readonly IWarehouseVendorService _warehouseVendorService;
		private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
		private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
		private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
		private readonly IDiscountService _discountService;
		private readonly IEventPublisher _eventPublisher;
		private readonly IVirtualProductMappingService _virtualProductService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IDelsamiProductModelFactory _DelsamiProductModelFactory;
		private readonly IGroupedProductStockService _groupedProductStockService;
		private readonly IVirtualProductMappingService _virtualProductMappingService;
		private readonly IBundleProductService _bundleProductService;
		private readonly ICategorySpecificationAttributeService _categorySpecificationAttributeService;
		private readonly ILogger _logger;


		#endregion

		#region Ctor

		public CustomProductController(IAclService aclService,
									   ICategorySpecificationAttributeService categorySpecificationAttributeService,
									   IImportReportService importReportService,
									   ProductManagementSettings productManagementSettings,
									   ICustomDelsamiProductService customProductService,
									   IVirtualProductMappingService virtualProductMappingService,
									   IGroupedProductStockService groupedProductStockService,
									   IBackInStockSubscriptionService backInStockSubscriptionService,
									   ICategoryService categoryService,
									   ICustomerService customerService,
									   ISpecificationAttributeService specificationAttributeService,
									   ICopyProductService copyProductService,
									   ICustomerActivityService customerActivityService,
									   IDiscountService discountService,
									   IDownloadService downloadService,
									   CoreSettings coreSettings,
									   IBundleProductService bundleProductService,
									   IExportManager exportManager,
									   IGenericAttributeService genericAttributeService,
									   IHttpClientFactory httpClientFactory,
									   IImportManager importManager,
									   ILanguageService languageService,
									   ILocalizationService localizationService,
									   ILocalizedEntityService localizedEntityService,
									   IManufacturerService manufacturerService,
									   INopFileProvider fileProvider,
									   INotificationService notificationService,
									   IPdfService pdfService,
									   IPermissionService permissionService,
									   IPictureService pictureService,
									   IProductAttributeFormatter productAttributeFormatter,
									   IProductAttributeParser productAttributeParser,
									   IProductAttributeService productAttributeService,
									   IProductModelFactory productModelFactory,
									   IProductService productService,
									   IRedirectionsService redirectionsService,
									   IProductTagService productTagService,
									   ISettingService settingService,
									   IShippingService shippingService,
									   IShoppingCartService shoppingCartService,
									   IStoreContext storeContext,
									   IUrlRecordService urlRecordService,
									   IVideoService videoService,
									   IWebHelper webHelper,
									   IWorkContext workContext,
									   VendorSettings vendorSettings,
									   IWarehouseVendorService warehouseVendorService,
									   IAclSupportedModelFactory aclSupportedModelFactory,
									   IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
									   IDiscountSupportedModelFactory discountSupportedModelFactory,
									   IEventPublisher eventPublisher,
									   IVirtualProductMappingService virtualProductService,
									   IDelsamiProductModelFactory DelsamiProductModelFactory,
									   ILogger logger) : base(aclService,
																			 backInStockSubscriptionService,
																			 categoryService,
																			 copyProductService,
																			 customerActivityService,
																			 customerService,
																			 discountService,
																			 downloadService,
																			 exportManager,
																			 genericAttributeService,
																			 httpClientFactory,
																			 importManager,
																			 languageService,
																			 localizationService,
																			 localizedEntityService,
																			 manufacturerService,
																			 fileProvider,
																			 notificationService,
																			 pdfService,
																			 permissionService,
																			 pictureService,
																			 productAttributeFormatter,
																			 productAttributeParser,
																			 productAttributeService,
																			 productModelFactory,
																			 productService,
																			 productTagService,
																			 settingService,
																			 shippingService,
																			 shoppingCartService,
																			 specificationAttributeService,
																			 storeContext,
																			 urlRecordService,
																			 videoService,
																			 webHelper,
																			 workContext,
																			 vendorSettings)
		{
			_workContext = workContext;
			_productManagementSettings = productManagementSettings;
			_categorySpecificationAttributeService = categorySpecificationAttributeService;
			_localizedEntityService = localizedEntityService;
			_customerService = customerService;
			_importReportService = importReportService;
			_specificationService = specificationAttributeService;
			_coreSettings = coreSettings;
			_redirectionsService = redirectionsService;
			_categoryService = categoryService;
			_exportManager = exportManager;
			_customProductService = customProductService;
			_vendorSettings = vendorSettings;
			_productService = productService;
			_notificationService = notificationService;
			_localizationService = localizationService;
			_bundleProductService = bundleProductService;
			_permissionService = permissionService;
			_productModelFactory = productModelFactory;
			_genericAttributeService = genericAttributeService;
			_urlRecordService = urlRecordService;
			_productTagService = productTagService;
			_customerActivityService = customerActivityService;
			_storeContext = storeContext;
			_backInStockSubscriptionService = backInStockSubscriptionService;
			_downloadService = downloadService;
			_shippingService = shippingService;
			_warehouseVendorService = warehouseVendorService;
			_aclSupportedModelFactory = aclSupportedModelFactory;
			_storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
			_discountSupportedModelFactory = discountSupportedModelFactory;
			_discountService = discountService;
			_eventPublisher = eventPublisher;
			_virtualProductService = virtualProductService;
			_DelsamiProductModelFactory = DelsamiProductModelFactory;
			_groupedProductStockService = groupedProductStockService;
			_virtualProductMappingService = virtualProductMappingService;
			_logger = logger;
		}

		#endregion

		#region Product

		[HttpGet]
		public override async Task<IActionResult> List()
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareProductSearchModelAsync(new DelsamiProductSearchModel());

			return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/List.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> List(DelsamiProductSearchModel searchModel)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return await AccessDeniedDataTablesJson();

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareProductListModelAsync(searchModel);

			return Json(model);
		}


		[HttpPost, ActionName("ExportToXml")]
		[FormValueRequired("exportxml-all")]
		public override async Task<IActionResult> ExportXmlAll(ProductSearchModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null)
			{
				model.SearchVendorId = currentVendor.Id;
			}

			var categoryIds = new List<int> { model.SearchCategoryId };
			//include subcategories
			if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
				categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

			//0 - all (according to "ShowHidden" parameter)
			//1 - published only
			//2 - unpublished only
			bool? overridePublished = null;
			if (model.SearchPublishedId == 1)
				overridePublished = true;
			else if (model.SearchPublishedId == 2)
				overridePublished = false;

			var products = await _customProductService.SearchProductsAsync(0,
				categoryIds: categoryIds,
				manufacturerIds: new List<int> { model.SearchManufacturerId },
				storeId: model.SearchStoreId,
				vendorId: model.SearchVendorId,
				warehouseId: model.SearchWarehouseId,
				productType: model.SearchProductTypeId > 0 ? (ProductTypes?)model.SearchProductTypeId : null,
				keywords: model.SearchProductName,
				showHidden: true,
				overridePublished: overridePublished);

			try
			{
				var xml = await _exportManager.ExportProductsToXmlAsync(products);

				return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
			}
			catch (Exception exc)
			{
				await _notificationService.ErrorNotificationAsync(exc);
				return RedirectToAction("List");
			}
		}

		[HttpPost, ActionName("ExportToExcel")]
		[FormValueRequired("exportexcel-all")]
		public override async Task<IActionResult> ExportExcelAll(ProductSearchModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null)
			{
				model.SearchVendorId = currentVendor.Id;
			}

			var categoryIds = new List<int> { model.SearchCategoryId };
			//include subcategories
			if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
				categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

			//0 - all (according to "ShowHidden" parameter)
			//1 - published only
			//2 - unpublished only
			bool? overridePublished = null;
			if (model.SearchPublishedId == 1)
				overridePublished = true;
			else if (model.SearchPublishedId == 2)
				overridePublished = false;

			var products = await _customProductService.SearchProductsAsync(0,
				categoryIds: categoryIds,
				manufacturerIds: new List<int> { model.SearchManufacturerId },
				storeId: model.SearchStoreId,
				vendorId: model.SearchVendorId,
				warehouseId: model.SearchWarehouseId,
				productType: model.SearchProductTypeId > 0 ? (ProductTypes?)model.SearchProductTypeId : null,
				keywords: model.SearchProductName,
				showHidden: true,
				overridePublished: overridePublished);

			try
			{
				var bytes = await _exportManager.ExportProductsToXlsxAsync(products);

				return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
			}
			catch (Exception exc)
			{
				await _notificationService.ErrorNotificationAsync(exc);

				return RedirectToAction("List");
			}
		}

		[HttpPost]
		public async Task<IActionResult> PreviewVirtualProducts(string virtualProducts, int mainProductId)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();


			var mainProduct = await _productService.GetProductByIdAsync(mainProductId);

			if (mainProduct is null)
				return RedirectToAction("List");

			var titles = virtualProducts
						.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
						.Select(x => x.Trim())
						.ToList();

			var existProduct = (await _customProductService.GetProductsByName(titles))?.Select(a => a.Name).ToHashSet() ?? new HashSet<string>();

			var model = new VirtualProductPreviewModel();
			foreach (var virtualProductName in titles)
			{
				var isExist = existProduct.Contains(virtualProductName);

				model.Items.Add(new VirtualProductPreviewItemModel
				{
					Selected = !isExist,
					CanSelect = !isExist,
					Name = virtualProductName,
					Description = isExist ? "عنوان تکراری" : ""
				});

			}
			return PartialView("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/_CreateOrUpdate.PreviewVirtualProducts.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> CreateVirtualProductsByLineBreak(VirtualProductPreviewModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			var mainProduct = await _productService.GetProductByIdAsync(model.MainProductId);

			if (mainProduct is null)
				return RedirectToAction("List");

			await _logger.InformationAsync($"Create vitual product by line break, the model is: {JsonConvert.SerializeObject(model)}");

			var mainProductModel = await _DelsamiProductModelFactory.PrepareDelsamiProductModelAsync(null, mainProduct);
			var parentOfMainProduct = await _productService.GetProductByIdAsync(mainProduct.ParentGroupedProductId);

			foreach (var item in model.Items.Where(a => a.Selected))
			{
				var virtualProduct = mainProduct.MyDeepClone();
				var parentOfvirtualProduct = parentOfMainProduct.MyDeepClone();

				parentOfvirtualProduct.CreatedOnUtc = DateTime.UtcNow;
				parentOfvirtualProduct.UpdatedOnUtc = DateTime.UtcNow;
				parentOfvirtualProduct.Name = item.Name;
				virtualProduct.Name = item.Name;
				virtualProduct.CreatedOnUtc = DateTime.UtcNow;
				virtualProduct.UpdatedOnUtc = DateTime.UtcNow;
				virtualProduct.Sku = null;
				virtualProduct.ManageInventoryMethod = ManageInventoryMethod.DontManageStock;
				virtualProduct.StockQuantity = 0;

				//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
				//DeliveryDateId is 'MainProductId'
				virtualProduct.AllowAddingOnlyExistingAttributeCombinations = true;
				virtualProduct.DeliveryDateId = mainProduct.Id;


				await _productService.InsertProductAsync(parentOfvirtualProduct);
				await _productService.InsertProductAsync(virtualProduct);

				virtualProduct.ParentGroupedProductId = parentOfvirtualProduct.Id;
				virtualProduct.Sku = virtualProduct.Id.ToString();
				await _productService.UpdateProductAsync(parentOfvirtualProduct);

				parentOfvirtualProduct.Sku = parentOfvirtualProduct.Id.ToString();
				await _productService.UpdateProductAsync(parentOfvirtualProduct);

				var pictures = await _productService.GetProductPicturesByProductIdAsync(parentOfMainProduct.Id) ?? new List<ProductPicture>();
				foreach (var picture in pictures)
				{
					picture.ProductId = parentOfvirtualProduct.Id;
					await _productService.InsertProductPictureAsync(picture);
				}

				//search engine name
				var seName = await _urlRecordService.ValidateSeNameAsync(parentOfvirtualProduct, item.Name, item.Name, true);
				await _urlRecordService.SaveSlugAsync(parentOfvirtualProduct, seName, 0);

				await UpdateLocalesAsync(virtualProduct, mainProductModel);

				await SaveCategoryMappingsAsync(virtualProduct, mainProductModel.SelectedCategoryIds);

				if (parentOfMainProduct is not null)
				{
					var categoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(parentOfMainProduct.Id)).Select(s => s.CategoryId).ToList();

					await SaveCategoryMappingsAsync(virtualProduct, categoryIds);
				}


				//manufacturers
				await SaveManufacturerMappingsAsync(virtualProduct, mainProductModel);

				//ACL (customer roles)
				await SaveProductAclAsync(virtualProduct, mainProductModel);

				//stores
				await _productService.UpdateProductStoreMappingsAsync(virtualProduct, mainProductModel.SelectedStoreIds);

				//discounts
				await SaveDiscountMappingsAsync(virtualProduct, mainProductModel);

				//tags
				await _productTagService.UpdateProductTagsAsync(virtualProduct, ParseProductTags(mainProductModel.ProductTags));

				//warehouses
				await SaveProductWarehouseInventoryAsync(virtualProduct, mainProductModel);

				//quantity change history
				await _productService.AddStockQuantityHistoryEntryAsync(virtualProduct, virtualProduct.StockQuantity, virtualProduct.StockQuantity, virtualProduct.WarehouseId,
					await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));

				await _virtualProductService.InsertVirtualProductAsync(new VirtualProductMapping
				{
					VirtualProductId = virtualProduct.Id,
					MainProductId = mainProduct.Id,
					Deleted = false
				});

			}


			return Ok();
		}

		public override async Task<IActionResult> Create(bool showtour = false)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			var currentVendor = await _workContext.GetCurrentVendorAsync();

			//validate maximum number of products per vendor
			if (_vendorSettings.MaximumProductNumber > 0 && currentVendor != null
			&& await _productService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumProductNumber)
			{
				_notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"),
					_vendorSettings.MaximumProductNumber));
				return RedirectToAction("List", "Product");
			}

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareDelsamiProductModelAsync(new DelsamiProductModel(), null);

			//show configuration tour
			if (showtour)
			{
				var customer = await _workContext.GetCurrentCustomerAsync();
				var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
				var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

				if (!hideCard && !closeCard)
					ViewBag.ShowTour = true;
			}

			if (model.IsLoggedInAsVendor)
			{
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/CreateForVendor.cshtml", model);
			}
			else
			{
				//model.VisibleIndividually = false;
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/Create.cshtml", model);
			}
		}


		[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public async Task<IActionResult> NewCreate(DelsamiProductModel model, bool continueEditing)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//validate maximum number of products per vendor
			var currentVendor = await _workContext.GetCurrentVendorAsync();

			if (_vendorSettings.MaximumProductNumber > 0 && currentVendor != null
				&& await _productService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumProductNumber)
			{
				_notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"),
					_vendorSettings.MaximumProductNumber));

				return RedirectToAction("List", "Product");
			}

			if (currentVendor != null)
			{
				var warehouseVendor = await _warehouseVendorService.GetWarehouseVendorByVendorIdAsync(currentVendor.Id);

				if (warehouseVendor == null)
					ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseForVendorIsNotSpecified"));

				model.WarehouseId = warehouseVendor?.WarehouseId ?? 0;
			}

			if (model.ProductTypeId == (int)ProductType.SimpleProduct && model.Published && (model.Height == 0 ||
																				 model.Weight == 0 ||
																				 model.Length == 0 ||
																				 model.Width == 0))
			{
				ModelState.AddModelError("", "");

				_notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.ProductWithoutDimensionsCannotPublished"));
			}

			if (ModelState.IsValid)
			{
				model.ManageInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

				if (currentVendor != null)
					model.Published = false;

				//a vendor should have access only to his products
				if (currentVendor != null)
					model.VendorId = currentVendor.Id;

				//vendors cannot edit "Show on home page" property
				if (currentVendor != null && model.ShowOnHomepage)
					model.ShowOnHomepage = false;

				if (model.ProductTypeId == (int)ProductType.GroupedProduct)
					model.VisibleIndividually = true;
				else
					model.VisibleIndividually = false;

				if (model.ProductTypeId == (int)ProductType.SimpleProduct)
				{
					model.IsShipEnabled = true;
					model.OrderMinimumQuantity = 1;
				}

				//product
				var product = model.ToEntity<Product>();
				product.CreatedOnUtc = DateTime.UtcNow;
				product.UpdatedOnUtc = DateTime.UtcNow;

				if (model.IsVirtualProduct)
				{
					model.Sku = null;
					product.ProductType = ProductType.SimpleProduct;
					product.ManageInventoryMethod = ManageInventoryMethod.DontManageStock;
					product.StockQuantity = 0;
					//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
					//DeliveryDateId is 'MainProductId'
					product.AllowAddingOnlyExistingAttributeCombinations = model.IsVirtualProduct;
					product.DeliveryDateId = model.MainProductId ?? 0;
				}



				await _productService.InsertProductAsync(product);

				if (product.ProductType == ProductType.GroupedProduct || model.IsVirtualProduct)
				{
					product.Sku = product.Id.ToString();

					await _productService.UpdateProductAsync(product);
				}
				//search engine name
				model.SeName = await _urlRecordService.ValidateSeNameAsync(product, model.SeName, product.Name, true);
				await _urlRecordService.SaveSlugAsync(product, model.SeName, 0);

				//locales
				await UpdateLocalesAsync(product, model);

				//categories

				//add all category to product
				if (!model.SelectedCategoryIds.Any(a => a == _coreSettings.AllProductsCategoryId))
				{
					model.SelectedCategoryIds.Add(_coreSettings.AllProductsCategoryId);
				}

				//await SaveCategoryMappingsAsync(product, model);
				if (product.ProductType == ProductType.GroupedProduct)
				{

					await SaveCategoryMappingsAsync(product, model.SelectedCategoryIds);

					var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id);

					foreach (var associatedProduct in associatedProducts)
					{
						await SaveCategoryMappingsAsync(associatedProduct, model.SelectedCategoryIds);
					}
				}
				else //simple product
				{
					await SaveCategoryMappingsAsync(product, model.SelectedCategoryIds);

					var parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

					if (parentProduct is not null)
					{
						var categoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(parentProduct.Id)).Select(s => s.CategoryId).ToList();

						await SaveCategoryMappingsAsync(product, categoryIds);
					}
				}

				//manufacturers
				await SaveManufacturerMappingsAsync(product, model);

				//ACL (customer roles)
				await SaveProductAclAsync(product, model);

				//stores
				await _productService.UpdateProductStoreMappingsAsync(product, model.SelectedStoreIds);

				//discounts
				await SaveDiscountMappingsAsync(product, model);

				//tags
				await _productTagService.UpdateProductTagsAsync(product, ParseProductTags(model.ProductTags));

				//warehouses
				await SaveProductWarehouseInventoryAsync(product, model);

				//quantity change history
				await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId,
					await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));

				//activity log
				await _customerActivityService.InsertActivityAsync("AddNewProduct",
					string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewProduct"), product.Name), product);

				if (model.IsVirtualProduct && product.ProductType != ProductType.GroupedProduct)
					await _virtualProductService.InsertVirtualProductAsync(new VirtualProductMapping { VirtualProductId = product.Id, Deleted = false });


				_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Added"));

				if (!continueEditing)
					return RedirectToAction("List", "Product");

				return RedirectToAction("Edit", new { id = product.Id });
			}


			//prepare model
			model = await _DelsamiProductModelFactory.PrepareDelsamiProductModelAsync(model, null, true);

			if (currentVendor != null)
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/CreateForVendor.cshtml", model);
			else
				//if we got this far, something failed, redisplay form
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/Create.cshtml", model);
		}


		public override async Task<IActionResult> Edit(int id)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null || product.Deleted)
				return RedirectToAction("List", "Product");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return RedirectToAction("List", "Product");

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareDelsamiProductModelAsync(null, product);

			if (model.IsLoggedInAsVendor)
			{
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/EditForVendor.cshtml", model);
			}
			else
			{
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/Edit.cshtml", model);
			}
		}


		[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public async Task<IActionResult> NewEdit(DelsamiProductModel model, bool continueEditing)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			DelsamiProductModel DelsamiProductModel = (DelsamiProductModel)model;

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(model.Id);
			if (product == null || product.Deleted)
				return RedirectToAction("List", "Product");


			//var oldProductStr = JsonSerializer.Serialize(product);
			//var oldProduct = JsonSerializer.Deserialize<Product>(oldProductStr);
			var oldProduct = product.MyDeepClone();

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();

			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return RedirectToAction("List", "Product");

			if (currentVendor != null)
				await PrepareProductModelForVendorAsync(product, model);
			//check if the product quantity has been changed while we were editing the product
			//and if it has been changed then we show error notification
			//and redirect on the editing page without data saving
			if (product.StockQuantity != model.LastStockQuantity && product.ProductType == ProductType.SimpleProduct && model.ProductTypeId == (int)ProductType.SimpleProduct)
			{
				_notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
				return RedirectToAction("Edit", new { id = product.Id });
			}

			if (currentVendor != null)
			{
				var warehouseVendor = await _warehouseVendorService.GetWarehouseVendorByVendorIdAsync(currentVendor.Id);

				if (warehouseVendor == null)
					ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseForVendorIsNotSpecified"));

				model.WarehouseId = warehouseVendor?.WarehouseId ?? 0;
			}

			if (model.ProductTypeId == (int)ProductType.SimpleProduct && model.Published && (model.Height == 0 ||
																							 model.Weight == 0 ||
																							 model.Length == 0 ||
																							 model.Width == 0))
			{
				ModelState.AddModelError("", "");

				_notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.ProductWithoutDimensionsCannotPublished"));
			}

			if (ModelState.IsValid)
			{
				model.ManageInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

				//a vendor should have access only to his products
				if (currentVendor != null)
				{
					model.VendorId = currentVendor.Id;
					model.Published = false;
				}

				//we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
				//vendors cannot edit "Show on home page" property
				if (currentVendor != null && model.ShowOnHomepage != product.ShowOnHomepage)
					model.ShowOnHomepage = product.ShowOnHomepage;

				//some previously used values
				var prevTotalStockQuantity = await _productService.GetTotalStockQuantityAsync(product);
				var prevDownloadId = product.DownloadId;
				var prevSampleDownloadId = product.SampleDownloadId;
				var previousStockQuantity = product.StockQuantity;
				var previousWarehouseId = product.WarehouseId;
				var previousProductType = product.ProductType;
				var mainProductId = product.DeliveryDateId;

				if (model.ProductTypeId == (int)ProductType.GroupedProduct)
				{
					model.VisibleIndividually = true;
				}
				else
				{
					model.VisibleIndividually = false;
					model.IsShipEnabled = true;
				}

				if (DelsamiProductModel.IsVirtualProduct)
				{
					model.Sku = null;
					model.ManageInventoryMethodId = (int)ManageInventoryMethod.DontManageStock;
					model.StockQuantity = 0;
					model.ProductTypeId = (int)ProductType.SimpleProduct;
				}

				//product
				product = model.ToEntity(product);

				product.DeliveryDateId = mainProductId;
				product.UpdatedOnUtc = DateTime.UtcNow;

				if (product.ProductType == ProductType.GroupedProduct || DelsamiProductModel.IsVirtualProduct)
				{
					if (product.ProductType == ProductType.GroupedProduct)
						product.VisibleIndividually = true;

					product.Sku = product.Id.ToString();
				}


				var virtualProductMapping = await _virtualProductService.GetVirtualProductMappingByVirtualProductIdAsync(model.Id);

				//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
				//DeliveryDateId is 'MainProductId'
				if (DelsamiProductModel.IsVirtualProduct && virtualProductMapping is null)
				{
					product.AllowAddingOnlyExistingAttributeCombinations = true;
					await _virtualProductService.InsertVirtualProductAsync(new VirtualProductMapping
					{
						VirtualProductId = model.Id,
					});
				}

				if ((!DelsamiProductModel.IsVirtualProduct || DelsamiProductModel.ProductTypeId == (int)ProductType.GroupedProduct) && virtualProductMapping is not null)
				{
					product.AllowAddingOnlyExistingAttributeCombinations = false;
					product.DeliveryDateId = 0;
					await _virtualProductService.DeleteVirtualProductAsync(virtualProductMapping);
				}

				await _productService.UpdateProductAsync(product);
				await _eventPublisher.PublishAsync(new AuditUpdatedEntityEvent<Product>(product, oldProduct));

				//remove associated products
				if (previousProductType == ProductType.GroupedProduct && product.ProductType == ProductType.SimpleProduct)
				{
					var store = await _storeContext.GetCurrentStoreAsync();
					var storeId = store?.Id ?? 0;
					var vendorId = currentVendor?.Id ?? 0;

					var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id, storeId, vendorId);
					foreach (var associatedProduct in associatedProducts)
					{
						associatedProduct.ParentGroupedProductId = 0;
						await _productService.UpdateProductAsync(associatedProduct);
					}
				}

				if (oldProduct.Published != product.Published)
				{
					var updatedParentIds = new HashSet<int>();

					var virtualMappings = await _customProductService.GetVirtualProductsByMainProductIdAsync(product.Id);

					foreach (var vp in virtualMappings)
					{
						var parentId = vp.ParentGroupedProductId;

						if (parentId == 0 || updatedParentIds.Contains(parentId))
							continue;

						updatedParentIds.Add(parentId);

						await _groupedProductStockService.RecalculateParentStockAsync(parentId);
					}

					if (product.ParentGroupedProductId != 0 &&
						!updatedParentIds.Contains(product.ParentGroupedProductId))
					{
						await _groupedProductStockService
							.RecalculateParentStockAsync(product.ParentGroupedProductId);
					}
				}


				var quantityDelta = product.StockQuantity - previousStockQuantity;

				if (quantityDelta != 0)
				{
					await _groupedProductStockService.UpdateParentStockIncrementalAsync(product.Id, quantityDelta);
				}

				//search engine name
				model.SeName = await _urlRecordService.ValidateSeNameAsync(product, model.SeName, product.Name, true);
				await _urlRecordService.SaveSlugAsync(product, model.SeName, 0);

				//locales
				await UpdateLocalesAsync(product, model);

				//tags
				await _productTagService.UpdateProductTagsAsync(product, ParseProductTags(model.ProductTags));

				//warehouses
				await SaveProductWarehouseInventoryAsync(product, model);

				//categories

				//add all category to product
				if (!model.SelectedCategoryIds.Any(a => a == _coreSettings.AllProductsCategoryId))
				{
					model.SelectedCategoryIds.Add(_coreSettings.AllProductsCategoryId);
				}

				//await SaveCategoryMappingsAsync(product, model);

				if (product.ProductType == ProductType.GroupedProduct)
				{
					await SaveCategoryMappingsAsync(product, model.SelectedCategoryIds);

					var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id);

					foreach (var associatedProduct in associatedProducts)
					{
						await SaveCategoryMappingsAsync(associatedProduct, model.SelectedCategoryIds);
					}
				}
				else //simple product
				{
					await SaveCategoryMappingsAsync(product, model.SelectedCategoryIds);

					var parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

					if (parentProduct is not null)
					{
						var categoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(parentProduct.Id)).Select(s => s.CategoryId).ToList();

						await SaveCategoryMappingsAsync(product, categoryIds);
					}
				}

				//manufacturers
				await SaveManufacturerMappingsAsync(product, model);

				//ACL (customer roles)
				await SaveProductAclAsync(product, model);

				//stores
				await _productService.UpdateProductStoreMappingsAsync(product, model.SelectedStoreIds);

				//discounts
				await SaveDiscountMappingsAsync(product, model);

				//picture seo names
				await UpdatePictureSeoNamesAsync(product);

				//back in stock notifications
				if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
					product.BackorderMode == BackorderMode.NoBackorders &&
					product.AllowBackInStockSubscriptions &&
					await _productService.GetTotalStockQuantityAsync(product) > 0 &&
					prevTotalStockQuantity <= 0 &&
					product.Published &&
					!product.Deleted)
				{
					await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(product);
				}

				//delete an old "download" file (if deleted or updated)
				if (prevDownloadId > 0 && prevDownloadId != product.DownloadId)
				{
					var prevDownload = await _downloadService.GetDownloadByIdAsync(prevDownloadId);
					if (prevDownload != null)
						await _downloadService.DeleteDownloadAsync(prevDownload);
				}

				//delete an old "sample download" file (if deleted or updated)
				if (prevSampleDownloadId > 0 && prevSampleDownloadId != product.SampleDownloadId)
				{
					var prevSampleDownload = await _downloadService.GetDownloadByIdAsync(prevSampleDownloadId);
					if (prevSampleDownload != null)
						await _downloadService.DeleteDownloadAsync(prevSampleDownload);
				}

				//quantity change history
				if (previousWarehouseId != product.WarehouseId)
				{
					//warehouse is changed 
					//compose a message
					var oldWarehouseMessage = string.Empty;
					if (previousWarehouseId > 0)
					{
						var oldWarehouse = await _shippingService.GetWarehouseByIdAsync(previousWarehouseId);
						if (oldWarehouse != null)
							oldWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
					}

					var newWarehouseMessage = string.Empty;
					if (product.WarehouseId > 0)
					{
						var newWarehouse = await _shippingService.GetWarehouseByIdAsync(product.WarehouseId);
						if (newWarehouse != null)
							newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
					}

					var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

					//record history
					await _productService.AddStockQuantityHistoryEntryAsync(product, -previousStockQuantity, 0, previousWarehouseId, message);
					await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
				}
				else
				{
					await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
						product.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));
				}

				//activity log
				await _customerActivityService.InsertActivityAsync("EditProduct",
					string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProduct"), product.Name), product);

				_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Updated"));

				if (!continueEditing)
					return RedirectToAction("List", "Product");

				return RedirectToAction("Edit", new { id = product.Id });
			}

			//prepare model
			model = await _DelsamiProductModelFactory.PrepareDelsamiProductModelAsync((DelsamiProductModel)model, product, true);

			if (model.IsLoggedInAsVendor)
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/EditForVendor.cshtml", model);
			else
				return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/Edit.cshtml", model);
		}

		public override async Task<IActionResult> Delete(int id)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
				return RedirectToAction("List");

			var currentCustomer = await _workContext.GetCurrentCustomerAsync();

			//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
			if (product.ProductType == ProductType.SimpleProduct && !product.AllowAddingOnlyExistingAttributeCombinations && !await _customerService.IsInCustomerRoleAsync(currentCustomer, "Administrators"))
			{
				_notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ProductManagement.Admin.Product.CannotDeleteSimpleProduct"));

				return RedirectToAction("List");
			}

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return RedirectToAction("List");

			await _productService.DeleteProductAsync(product);

			await _groupedProductStockService.RecalculateParentStockAsync(product.ParentGroupedProductId);


			var virtualProductMapping = await _virtualProductService.GetVirtualProductMappingByVirtualProductIdAsync(product.Id);

			if (virtualProductMapping != null)
			{
				var virtualProduct = await _productService.GetProductByIdAsync(virtualProductMapping.VirtualProductId);

				if (virtualProduct != null)
				{
					virtualProduct.DeliveryDateId = 0;
					await _productService.DeleteProductAsync(virtualProduct);

					var parentOfVirtualProduct = await _productService.GetProductByIdAsync(virtualProduct.ParentGroupedProductId);

					if (parentOfVirtualProduct != null && !parentOfVirtualProduct.Deleted && parentOfVirtualProduct.Published)
					{
						parentOfVirtualProduct.Published = false;

						await _productService.UpdateProductAsync(parentOfVirtualProduct);
					}
				}

				await _virtualProductService.DeleteVirtualProductAsync(virtualProductMapping);
			}

			//activity log
			await _customerActivityService.InsertActivityAsync("DeleteProduct",
				string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProduct"), product.Name), product);

			_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Deleted"));

			return RedirectToAction("List");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteGroupProduct(int id, bool ChangeRedirect, int? RedirectProductId)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
				return RedirectToAction("List");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return RedirectToAction("List");

			await _productService.DeleteProductAsync(product);

			if (ChangeRedirect && RedirectProductId != null && RedirectProductId > 0)
			{
				var RedirectedProduct = await _productService.GetProductByIdAsync(RedirectProductId.Value) ?? throw new NullReferenceException();

				var pattern = await _urlRecordService.GetSeNameAsync(id, "Product") ?? throw new NullReferenceException();
				var redirectUrl = await _urlRecordService.GetSeNameAsync(RedirectProductId.Value, "Product") ?? throw new NullReferenceException();
				var entity = new RedirectionRule()
				{
					StoreId = 0,
					Type = (int)RedirectionTypeEnum.Match,
					UseQueryString = false,
					RedirectUrl = redirectUrl.Insert(0, "/"),
					Pattern = pattern.Insert(0, "/"),

				};

				InsertRedirectionResult result = await _redirectionsService.InsertRedirectionsAsync(entity);

				if (result != InsertRedirectionResult.Ok)
					_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.DeletedWithRedirectFailed") + $"خطا : {result}");

			}

			//activity log
			await _customerActivityService.InsertActivityAsync("DeleteProductWithRedirect",
				string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductWithRedirect"), product.Name), product);



			_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Deleted"));

			return RedirectToAction("List");
		}

		public override async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			if (selectedIds == null || selectedIds.Count == 0)
				return NoContent();

			var currentVendor = await _workContext.GetCurrentVendorAsync();
			var products = (await _productService.GetProductsByIdsAsync(selectedIds.ToArray()))
							 .Where(p => currentVendor == null || p.VendorId == currentVendor.Id)
							 .ToList();


			var updatedParentIds = new HashSet<int>();
			var currentCustomer = await _workContext.GetCurrentCustomerAsync();

			foreach (var product in products)
			{
				//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
				if (product.ProductType == ProductType.SimpleProduct && !product.AllowAddingOnlyExistingAttributeCombinations && !await _customerService.IsInCustomerRoleAsync(currentCustomer, "Administrators"))
					continue;

				product.DeliveryDateId = 0;
				await _productService.DeleteProductAsync(product);

				if (product.ProductTypeId == (int)ProductTypes.GroupedProduct)
					continue;

				if (product.ParentGroupedProductId != 0 || !updatedParentIds.Contains(product.ParentGroupedProductId))
					updatedParentIds.Add(product.ParentGroupedProductId);

				var virtualProductMapping = await _virtualProductMappingService.GetVirtualProductMappingByVirtualProductIdAsync(product.Id);

				if (virtualProductMapping != null)
				{
					var virtualProduct = await _productService.GetProductByIdAsync(virtualProductMapping.VirtualProductId);

					if (virtualProduct != null)
					{
						virtualProduct.DeliveryDateId = 0;
						await _productService.DeleteProductAsync(virtualProduct);

						var parentOfVirtualProduct = await _productService.GetProductByIdAsync(virtualProduct.ParentGroupedProductId);

						if (parentOfVirtualProduct != null && !parentOfVirtualProduct.Deleted && parentOfVirtualProduct.Published)
						{
							parentOfVirtualProduct.Published = false;

							await _productService.UpdateProductAsync(parentOfVirtualProduct);
						}
					}

					await _virtualProductService.DeleteVirtualProductAsync(virtualProductMapping);

				}

			}


			foreach (var parentId in updatedParentIds)
				await _groupedProductStockService.RecalculateParentStockAsync(parentId);

			return Json(new { Result = true });
		}
		#endregion

		#region Virtual products

		[HttpPost]
		public virtual async Task<IActionResult> VirtualProductList(VirtualProductSearchModel searchModel)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return await AccessDeniedDataTablesJson();

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
				?? throw new ArgumentException("No product found with the specified id");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return Content("This is not your product");

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareVirtualProductListModelAsync(searchModel, product);

			return Json(model);
		}

		//[HttpPost]
		//public virtual async Task<IActionResult> VirtualProductDelete(int id)
		//{
		//    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
		//        return AccessDeniedView();

		//    //try to get a cross-sell product with the specified id
		//    var virtualProduct = await _virtualProductService.GetVirtualProductAsync(id)
		//        ?? throw new ArgumentException("No virtual product found with the specified id");

		//    //a vendor should have access only to his products
		//    var currentVendor = await _workContext.GetCurrentVendorAsync();
		//    if (currentVendor != null)
		//    {
		//        var product = await _productService.GetProductByIdAsync(virtualProduct.MainProductId.Value);
		//        if (product != null && product.VendorId != currentVendor.Id)
		//            return Content("This is not your product");
		//    }

		//    await _virtualProductService.DeleteVirtualProductFromMainAsync(virtualProduct);

		//    return new NullJsonResult();
		//}

		public virtual async Task<IActionResult> VirtualProductAddPopup(int productId)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareAddVirtualProductSearchModelAsync(new AddVirtualProductSearchModel());

			return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/VirtualProductAddPopup.cshtml", model);
		}

		[HttpPost]
		public virtual async Task<IActionResult> VirtualProductAddPopupList(AddVirtualProductSearchModel searchModel)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return await AccessDeniedDataTablesJson();

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareAddVirtualProductListModelAsync(searchModel);

			return Json(model);
		}

		[HttpPost]
		[FormValueRequired("save")]
		public virtual async Task<IActionResult> VirtualProductAddPopup(AddVirtualProductModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
			if (selectedProducts.Any())
			{
				var currentVendor = await _workContext.GetCurrentVendorAsync();
				foreach (var product in selectedProducts)
				{
					//a vendor should have access only to his products
					if (currentVendor != null && product.VendorId != currentVendor.Id)
						continue;

					var virtualProduct = await _virtualProductService.GetVirtualProductMappingByVirtualProductIdAsync(product.Id);

					if (virtualProduct == null || virtualProduct.MainProductId == model.MainProductId)
						continue;

					virtualProduct.MainProductId = model.MainProductId;

					await _virtualProductService.UpdateVirtualProductAsync(virtualProduct);

					//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
					//DeliveryDateId is 'MainProductId'
					product.AllowAddingOnlyExistingAttributeCombinations = true;
					product.DeliveryDateId = model.MainProductId;
					await _productService.UpdateProductAsync(product);
				}
			}

			var mainProduct = await _productService.GetProductByIdAsync(model.MainProductId);
			ViewBag.RefreshPage = true;

			return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/VirtualProductAddPopup.cshtml", new AddVirtualProductSearchModel());
		}

		#endregion

		#region Associated products

		[HttpPost]
		public override async Task<IActionResult> AssociatedProductDelete(int id)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get an associated product with the specified id
			var product = await _productService.GetProductByIdAsync(id)
				?? throw new ArgumentException("No associated product found with the specified id");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return Content("This is not your product");

			var parentGroupedProductId = product.ParentGroupedProductId;

			product.ParentGroupedProductId = 0;

			//delete old categories
			var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

			foreach (var productCategory in productCategories)
			{
				await _categoryService.DeleteProductCategoryAsync(productCategory);
			}

			await _productService.UpdateProductAsync(product);

			await _groupedProductStockService.RecalculateParentStockAsync(parentGroupedProductId);

			//Remove All Relations to Specs
			var relations = await _specificationService.GetProductSpecificationAttributesAsync(productId: product.Id);
			if (relations is not null && relations.Any())
			{
				foreach (var rel in relations)
				{
					await _specificationService.DeleteProductSpecificationAttributeAsync(rel);
				}

			}
			return new NullJsonResult();
		}

		public override async Task<IActionResult> AssociatedProductAddPopup(int productId)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//prepare model
			var model = await _productModelFactory.PrepareAddAssociatedProductSearchModelAsync(new AddAssociatedProductSearchModel());

			return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/AssociatedProductAddPopup.cshtml", model);
		}

		[HttpPost]
		[FormValueRequired("save")]
		public override async Task<IActionResult> AssociatedProductAddPopup(AddAssociatedProductModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());

			var tryToAddSelfGroupedProduct = selectedProducts
				.Select(p => p.Id)
				.Contains(model.ProductId);

			if (selectedProducts.Any())
			{
				bool hasRelations = false;
				var parentRelations = await _specificationService.GetProductSpecificationAttributesAsync(productId: model.ProductId);
				if (parentRelations is not null && parentRelations.Any())
					hasRelations = true;

				foreach (var product in selectedProducts)
				{
					if (product.Id == model.ProductId)
						continue;

					//a vendor should have access only to his products
					var currentVendor = await _workContext.GetCurrentVendorAsync();
					if (currentVendor != null && product.VendorId != currentVendor.Id)
						continue;

					product.ParentGroupedProductId = model.ProductId;

					//delete old categories
					var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

					foreach (var productCategory in productCategories)
					{
						await _categoryService.DeleteProductCategoryAsync(productCategory);
					}


					var parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

					if (parentProduct is not null)
					{
						var categoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(parentProduct.Id, true)).Select(s => s.CategoryId).ToList();

						await SaveCategoryMappingsAsync(product, categoryIds);
					}

					await _productService.UpdateProductAsync(product);

					//Add all relations to specs inherit from parent
					if (hasRelations)
						foreach (var rel in parentRelations)
						{
							var entity = new ProductSpecificationAttribute()
							{
								DisplayOrder = 0,
								AttributeTypeId = 0,
								ShowOnProductPage = true,
								AllowFiltering = true,
								SpecificationAttributeOptionId = rel.SpecificationAttributeOptionId,
								ProductId = product.Id,
							};
							await _specificationService.InsertProductSpecificationAttributeAsync(entity);
						}

				}
			}


			//if(model.)
			await _groupedProductStockService.RecalculateParentStockAsync(model.ProductId);


			if (tryToAddSelfGroupedProduct)
			{
				_notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.AssociatedProducts.TryToAddSelfGroupedProduct"));

				var addAssociatedProductSearchModel = await _productModelFactory.PrepareAddAssociatedProductSearchModelAsync(new AddAssociatedProductSearchModel());
				//set current product id
				addAssociatedProductSearchModel.ProductId = model.ProductId;

				ViewBag.RefreshPage = true;

				return View(addAssociatedProductSearchModel);
			}

			ViewBag.RefreshPage = true;

			ViewBag.ClosePage = true;

			return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/AssociatedProductAddPopup.cshtml", new AddAssociatedProductSearchModel());
		}



		[HttpGet]
		public async Task<IActionResult> ProductLookup(string term, int productId)
		{
			var products = (await _productService.SearchProductsAsync(pageSize: 50, keywords: term, productType: ProductType.GroupedProduct))
				.Select(p => new
				{
					Value = p.Id,
					Text = p.Name,
					Selected = false
				})
				.ToList();

			if (string.IsNullOrEmpty(term))
			{

				var childProducts = await _productService.GetAssociatedProductsAsync(productId);

				//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
				//DeliveryDateId is 'MainProductId'
				var isVirtual = childProducts.All(a => a.AllowAddingOnlyExistingAttributeCombinations);

				if (isVirtual)
				{
					var mainProduct = await _productService.GetProductByIdAsync(childProducts.First().Id);
					if (mainProduct is not null && !mainProduct.Deleted)
					{
						var parentProduct = await _productService.GetProductByIdAsync(mainProduct.ParentGroupedProductId);

						if (parentProduct is not null && !parentProduct.Deleted)
							products.Insert(0, new
							{
								Value = parentProduct.Id,
								Text = parentProduct.Name,
								Selected = true
							});
					}
				}

			}

			return Json(products);
		}
		#endregion

		#region Bundle product 


		[HttpPost]
		public virtual async Task<IActionResult> BundleProductList(BundleProductSearchModel searchModel)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return await AccessDeniedDataTablesJson();

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
				?? throw new ArgumentException("No product found with the specified id");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return Content("This is not your product");

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareBundleProductListModelAsync(searchModel, product);

			return Json(model);
		}

		[HttpPost]
		public virtual async Task<IActionResult> BundleProductUpdate(BundleProductModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a Bundle Product with the specified id
			var bundleProduct = await _bundleProductService.GetBundleProductByIdAsync(model.Id)
				?? throw new ArgumentException("No Bundle Product found with the specified id");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null)
			{
				var product = await _productService.GetProductByIdAsync(bundleProduct.ProductId1);
				if (product != null && product.VendorId != currentVendor.Id)
					return Content("This is not your product");
			}

			bundleProduct.Count = model.Count;
			bundleProduct.Price = model.Price;
			await _bundleProductService.UpdateBundleProductAsync(bundleProduct);

			return new NullJsonResult();
		}


		[HttpPost]
		public virtual async Task<IActionResult> BundleProductDelete(int id)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a Bundle product with the specified id
			var bundleProduct = await _bundleProductService.GetBundleProductByIdAsync(id)
				?? throw new ArgumentException("No Bundle product found with the specified id");

			var productId = bundleProduct.ProductId1;

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null)
			{
				var product = await _productService.GetProductByIdAsync(productId);
				if (product != null && product.VendorId != currentVendor.Id)
					return Content("This is not your product");
			}

			await _bundleProductService.DeleteBundleProductAsync(bundleProduct);

			return new NullJsonResult();
		}



		public virtual async Task<IActionResult> BundleProductAddPopup(int productId)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareAddBundleProductSearchModelAsync(new AddBundleProductSearchModel());
			model.ProductId = productId;

			return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/BundleProductAddPopup.cshtml", model);
		}

		[HttpPost]
		public virtual async Task<IActionResult> BundleProductAddPopupList(AddBundleProductSearchModel searchModel)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return await AccessDeniedDataTablesJson();

			//prepare model
			var model = await _DelsamiProductModelFactory.PrepareAddBundleProductListModelAsync(searchModel);

			return Json(model);
		}



		[HttpPost]
		[FormValueRequired("save")]
		public virtual async Task<IActionResult> BundleAddPopup(AddBundleProductModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
			if (selectedProducts.Any())
			{
				var existingBundleProducts = await _bundleProductService.GetBundleProductsByProductId1Async(model.ProductId, showHidden: true);
				var currentVendor = await _workContext.GetCurrentVendorAsync();
				foreach (var product in selectedProducts)
				{
					//a vendor should have access only to his products
					if (currentVendor != null && product.VendorId != currentVendor.Id)
						continue;

					if (_bundleProductService.FindBundleProduct(existingBundleProducts, model.ProductId, product.Id) != null)
						continue;

					await _bundleProductService.InsertBundleProductAsync(new BundleProduct
					{
						ProductId1 = model.ProductId,
						ProductId2 = product.Id,
						Count = model.Count,
					});
				}
			}

			ViewBag.RefreshPage = true;

			return View(
				"~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/Product/BundleProductAddPopup.cshtml",
				new AddBundleProductSearchModel());
		}

		#endregion

		#region Product specification attributes

		[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public override async Task<IActionResult> ProductSpecificationAttributeAdd(AddSpecificationAttributeModel model, bool continueEditing)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			var product = await _productService.GetProductByIdAsync(model.ProductId);
			if (product == null)
			{
				_notificationService.ErrorNotification("No product found with the specified id");
				return RedirectToAction("List");
			}

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
			{
				return RedirectToAction("List");
			}
			var associatedProducts = await _productService.GetAssociatedProductsAsync(model.ProductId);
			associatedProducts.Add(product);

			foreach (var item in associatedProducts)
			{
				ProductSpecificationAttribute entity = new();
				//we allow filtering only for "Option" attribute type
				if (model.AttributeTypeId != (int)SpecificationAttributeType.Option)
					entity.AllowFiltering = false;

				//we don't allow CustomValue for "Option" attribute type
				if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
					entity.CustomValue = null;

				//store raw html if field allow this
				if (model.AttributeTypeId == (int)SpecificationAttributeType.CustomText
					|| model.AttributeTypeId == (int)SpecificationAttributeType.Hyperlink)
					entity.CustomValue = model.Value;

				entity.ProductId = item.Id;
				entity.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;
				entity.CustomValue = !string.IsNullOrWhiteSpace(model.ValueRaw) && model.ValueRaw.StartsWith("<p>") ? null : model.ValueRaw;
				entity.AllowFiltering = model.AllowFiltering;
				entity.ShowOnProductPage = model.ShowOnProductPage;

				await _specificationService.InsertProductSpecificationAttributeAsync(entity);

				switch (entity.AttributeType)
				{
					case SpecificationAttributeType.CustomText:
						foreach (var localized in model.Locales)
						{
							await _localizedEntityService.SaveLocalizedValueAsync(entity,
								x => x.CustomValue,
								localized.Value,
								localized.LanguageId);
						}

						break;
					case SpecificationAttributeType.CustomHtmlText:
						foreach (var localized in model.Locales)
						{
							await _localizedEntityService.SaveLocalizedValueAsync(entity,
								x => x.CustomValue,
								localized.ValueRaw,
								localized.LanguageId);
						}

						break;
					case SpecificationAttributeType.Option:
						break;
					case SpecificationAttributeType.Hyperlink:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

			}

			if (continueEditing)
				return RedirectToAction("DelsamiProductSpecAttributeAddOrEdit",
					new { productId = model.ProductId/*, specificationId = psa.Id */});

			//select an appropriate card
			SaveSelectedCardName("product-specification-attributes");
			return RedirectToAction("Edit", new { id = model.ProductId });
		}


		[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public override async Task<IActionResult> ProductSpecAttrUpdate(AddSpecificationAttributeModel model, bool continueEditing)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a product specification attribute with the specified id
			var psa = await _specificationService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
			if (psa == null && model.SpecificationId != 0)
			{
				//select an appropriate card
				SaveSelectedCardName("product-specification-attributes");
				_notificationService.ErrorNotification("No product specification attribute found with the specified id");

				return RedirectToAction("Edit", new { id = model.ProductId });
			}

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null
				&& (await _productService.GetProductByIdAsync(psa.ProductId)).VendorId != currentVendor.Id)
			{
				_notificationService.ErrorNotification("This is not your product");

				return RedirectToAction("List");
			}

			if (model.SpecificationId == 0 && model.SpecificationAttributeOptionId == -1)
			{
				return RedirectToAction("Edit", new { id = model.ProductId });

			}

			if (model.SpecificationId == 0 && model.SpecificationAttributeOptionId > 0)
			{
				return await ProductSpecificationAttributeAdd(model, continueEditing);
			}

			if (model.SpecificationId != 0 && model.SpecificationAttributeOptionId == -1)
			{
				var productspec = await _specificationService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
				model.SpecificationAttributeOptionId = productspec.SpecificationAttributeOptionId;
				return await ProductSpecAttrDelete(model);
			}

			var associatedProducts = await _productService.GetAssociatedProductsAsync(model.ProductId);
			associatedProducts.Add(new Product()
			{
				Id = model.ProductId,
			});

			foreach (var product in associatedProducts)
			{
				var productSpecifications = await _specificationService.GetProductSpecificationAttributesAsync(product.Id, psa.SpecificationAttributeOptionId);
				foreach (var entity in productSpecifications)
				{

					//we allow filtering and change option only for "Option" attribute type
					//save localized values for CustomHtmlText and CustomText
					switch (model.AttributeTypeId)
					{
						case (int)SpecificationAttributeType.Option:
							entity.AllowFiltering = model.AllowFiltering;
							entity.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;

							break;
						case (int)SpecificationAttributeType.CustomHtmlText:
							entity.CustomValue = model.ValueRaw;
							foreach (var localized in model.Locales)
							{
								await _localizedEntityService.SaveLocalizedValueAsync(entity,
									x => x.CustomValue,
									localized.ValueRaw,
									localized.LanguageId);
							}

							break;
						case (int)SpecificationAttributeType.CustomText:
							entity.CustomValue = model.Value;
							foreach (var localized in model.Locales)
							{
								await _localizedEntityService.SaveLocalizedValueAsync(entity,
									x => x.CustomValue,
									localized.Value,
									localized.LanguageId);
							}

							break;
						default:
							entity.CustomValue = model.Value;

							break;
					}

					entity.ShowOnProductPage = model.ShowOnProductPage;
					entity.DisplayOrder = model.DisplayOrder;
					await _specificationService.UpdateProductSpecificationAttributeAsync(entity);
				}

			}

			if (continueEditing)
			{
				return RedirectToAction("DelsamiProductSpecAttributeAddOrEdit",
					new { productId = psa.ProductId, specificationId = model.SpecificationId });
			}

			//select an appropriate card
			SaveSelectedCardName("product-specification-attributes");

			return RedirectToAction("Edit", new { id = psa.ProductId });
		}

		[HttpPost]
		public override async Task<IActionResult> ProductSpecAttrList(ProductSpecificationAttributeSearchModel searchModel)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return await AccessDeniedDataTablesJson();

			//try to get a product with the specified id
			var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
				?? throw new ArgumentException("No product found with the specified id");

			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
				return Content("This is not your product");

			//prepare model
			//get product specification attributes
			var productSpecificationAttributes = await _specificationService.GetProductSpecificationAttributesAsync(product.Id);
			var list = new List<ProductSpecificationAttribute>();
			list.AddRange(productSpecificationAttributes);
			var categoryids = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).Select(a => a.CategoryId).ToList();
			var categorySpecs = await _categorySpecificationAttributeService.GetByCategoryIds(categoryids);
			var specOptions = await _specificationService.GetSpecificationAttributeOptionsByIdsAsync(productSpecificationAttributes.Select(a => a.SpecificationAttributeOptionId).ToArray());
			var specOptionsDic = specOptions.DistinctBy(a => a.SpecificationAttributeId).ToDictionary(a => a.SpecificationAttributeId, null);

			var unselectedSpecs = categorySpecs.Where(item => !specOptionsDic.ContainsKey(item.SpecificationAttributeId)).ToList();

			foreach (var spec in unselectedSpecs)
			{
				list.Add(new ProductSpecificationAttribute()
				{
					CustomValue = spec.SpecificationAttributeId.ToString(),
					AllowFiltering = true,
					DisplayOrder = 1,
					ProductId = product.Id,
					ShowOnProductPage = false,
					SpecificationAttributeOptionId = -1,// specAttribDic.TryGetValue(spec.SpecificationAttributeId, out var specOption) ? specOption.Id : 0,
					AttributeTypeId = 0,

				});
			}



			var productSpecificationAttributesPaged = list.ToPagedList(searchModel);
			//prepare grid model
			var model = await new ProductSpecificationAttributeListModel().PrepareToGridAsync(searchModel, productSpecificationAttributesPaged, () =>
			{
				return productSpecificationAttributesPaged.SelectAwait(async attribute =>
				{
					//fill in model values from the entity
					var productSpecificationAttributeModel = attribute.ToModel<ProductSpecificationAttributeModel>();

					var specAttributeOption = attribute.SpecificationAttributeOptionId != -1 ?
						await _specificationService.GetSpecificationAttributeOptionByIdAsync(attribute.SpecificationAttributeOptionId) :
						new SpecificationAttributeOption()
						{
							SpecificationAttributeId = int.Parse(attribute.CustomValue),
							Name = "انتخاب نشده",
						};


					var specAttribute = await _specificationService
						.GetSpecificationAttributeByIdAsync(specAttributeOption.SpecificationAttributeId);

					//fill in additional values (not existing in the entity)
					productSpecificationAttributeModel.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);

					productSpecificationAttributeModel.AttributeId = specAttribute.Id;
					productSpecificationAttributeModel.AttributeName = specAttribute.Name;// await DelsamiGetSpecificationAttributeNameAsync(specAttribute);
					var currentLanguage = await _workContext.GetWorkingLanguageAsync();

					switch (attribute.AttributeType)
					{
						case SpecificationAttributeType.Option:
							productSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
							productSpecificationAttributeModel.SpecificationAttributeOptionId = specAttributeOption.Id;
							break;
						case SpecificationAttributeType.CustomText:
							productSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id));
							break;
						case SpecificationAttributeType.CustomHtmlText:
							productSpecificationAttributeModel.ValueRaw = await _localizationService
								.GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id);
							break;
						case SpecificationAttributeType.Hyperlink:
							productSpecificationAttributeModel.ValueRaw = attribute.CustomValue;
							break;
					}

					return productSpecificationAttributeModel;
				});
			});

			return Json(model);
		}

		[HttpPost]
		public override async Task<IActionResult> ProductSpecAttrDelete(AddSpecificationAttributeModel model)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			//try to get a product specification attribute with the specified id
			var psa = await _specificationService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
			if (psa == null)
			{
				//select an appropriate card
				SaveSelectedCardName("product-specification-attributes");
				_notificationService.ErrorNotification("No product specification attribute found with the specified id");
				return RedirectToAction("Edit", new { id = model.ProductId });
			}

			var product = await _productService.GetProductByIdAsync(psa.ProductId);
			if (product == null)
			{
				_notificationService.ErrorNotification("No product found with the specified id");
				return RedirectToAction("List", new { id = model.ProductId });
			}
			//a vendor should have access only to his products
			var currentVendor = await _workContext.GetCurrentVendorAsync();
			if (currentVendor != null && product.VendorId != currentVendor.Id)
			{
				_notificationService.ErrorNotification("This is not your product");
				return RedirectToAction("List", new { id = model.ProductId });
			}

			var associatedProducts = await _productService.GetAssociatedProductsAsync(model.ProductId);
			associatedProducts.Add(product);

			foreach (var item in associatedProducts)
			{
				var productSpecAttrib = await _specificationService.GetProductSpecificationAttributesAsync(productId: item.Id, specificationAttributeOptionId: model.SpecificationAttributeOptionId);
				foreach (var spec in productSpecAttrib)
				{
					await _specificationService.DeleteProductSpecificationAttributeAsync(spec);

				}


			}

			//select an appropriate card
			SaveSelectedCardName("product-specification-attributes");

			return RedirectToAction("Edit", new { id = psa.ProductId });
		}

		public async Task<IActionResult> DelsamiProductSpecAttributeAddOrEdit(int productId, int? specificationId, int specAttribId, bool isSelected)
		{
			if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			if (await _productService.GetProductByIdAsync(productId) == null)
			{
				_notificationService.ErrorNotification("No product found with the specified id");
				return RedirectToAction("List");
			}

			//try to get a product specification attribute with the specified id
			try
			{
				var model = await _DelsamiProductModelFactory.PrepareAddSpecificationAttributeModelAsync(productId, specificationId, specAttribId, isSelected);
				return View("~/Areas/Admin/Views/Product/ProductSpecAttributeAddOrEdit.cshtml", model);
			}
			catch (Exception ex)
			{
				await _notificationService.ErrorNotificationAsync(ex);

				//select an appropriate card
				SaveSelectedCardName("product-specification-attributes");
				return RedirectToAction("Edit", new { id = productId });
			}
		}
		#endregion

		#region Utilities

		private async Task PrepareProductModelForVendorAsync(Product product, ProductModel model)
		{
			model.Published = product.Published;
			model.ProductTags = string.Join(", ", (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Select(tag => tag.Name));
			model.Gtin = product.Gtin;
			model.ManufacturerPartNumber = product.ManufacturerPartNumber;
			model.ShowOnHomepage = product.ShowOnHomepage;
			model.DisplayOrder = product.DisplayOrder;
			model.ProductTypeId = product.ProductTypeId;
			model.ProductTemplateId = product.ProductTemplateId;
			model.VisibleIndividually = product.VisibleIndividually;

			await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(model, product, false);

			await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, product, false);

			model.VendorId = product.VendorId;
			model.RequiredProductIds = product.RequiredProductIds;
			model.AllowCustomerReviews = product.AllowCustomerReviews;
			model.AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc;
			model.AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc;
			model.MarkAsNew = product.MarkAsNew;
			model.AdminComment = product.AdminComment;
			model.OldPrice = product.OldPrice;
			model.ProductCost = product.ProductCost;
			model.DisableBuyButton = product.DisableBuyButton;
			model.DisableWishlistButton = product.DisableWishlistButton;
			model.AvailableForPreOrder = product.AvailableForPreOrder;
			model.CallForPrice = product.CallForPrice;
			model.CustomerEntersPrice = product.CustomerEntersPrice;
			model.BasepriceEnabled = product.BasepriceEnabled;

			var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);

			await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, product, availableDiscounts, false);

			model.IsTaxExempt = product.IsTaxExempt;
			model.TaxCategoryId = product.TaxCategoryId;
			model.IsTelecommunicationsOrBroadcastingOrElectronicServices = product.IsTelecommunicationsOrBroadcastingOrElectronicServices;
			model.IsShipEnabled = product.IsShipEnabled;
			model.Weight = product.Weight;
			model.Length = product.Length;
			model.Width = product.Width;
			model.Height = product.Height;
			model.IsFreeShipping = product.IsFreeShipping;
			model.ShipSeparately = product.ShipSeparately;
			model.AdditionalShippingCharge = product.AdditionalShippingCharge;
			model.DeliveryDateId = product.DeliveryDateId;
			model.ManageInventoryMethodId = product.ManageInventoryMethodId;
			model.StockQuantity = product.StockQuantity;
			model.WarehouseId = product.WarehouseId;
			model.UseMultipleWarehouses = product.UseMultipleWarehouses;
			model.DisplayStockAvailability = product.DisplayStockAvailability;
			model.MinStockQuantity = product.MinStockQuantity;
			model.LowStockActivityId = product.LowStockActivityId;
			model.NotifyAdminForQuantityBelow = product.NotifyAdminForQuantityBelow;
			model.BackorderModeId = product.BackorderModeId;
			model.AllowBackInStockSubscriptions = product.AllowBackInStockSubscriptions;
			model.ProductAvailabilityRangeId = product.ProductAvailabilityRangeId;
			model.OrderMinimumQuantity = product.OrderMinimumQuantity;
			model.OrderMaximumQuantity = product.OrderMaximumQuantity;
			model.AllowedQuantities = product.AllowedQuantities;
			model.NotReturnable = product.NotReturnable;
			model.IsGiftCard = product.IsGiftCard;
			model.GiftCardTypeId = product.GiftCardTypeId;
			model.OverriddenGiftCardAmount = product.OverriddenGiftCardAmount;
			model.IsDownload = product.IsDownload;
			model.DownloadId = product.DownloadId;
			model.UnlimitedDownloads = product.UnlimitedDownloads;
			model.MaxNumberOfDownloads = product.MaxNumberOfDownloads;
			model.DownloadExpirationDays = product.DownloadExpirationDays;
			model.DownloadActivationTypeId = product.DownloadActivationTypeId;
			model.HasUserAgreement = product.HasUserAgreement;
			model.HasSampleDownload = product.HasSampleDownload;
			model.IsRecurring = product.IsRecurring;
			model.RecurringCycleLength = product.RecurringCycleLength;
			model.RecurringCyclePeriodId = product.RecurringCyclePeriodId;
			model.RecurringTotalCycles = product.RecurringTotalCycles;
			model.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
			model.MetaTitle = product.MetaTitle;
			model.MetaKeywords = product.MetaKeywords;
			model.MetaDescription = product.MetaDescription;

		}

		protected async Task SaveCategoryMappingsAsync(Product product, IList<int> selectedCategoryIds)
		{
			var existingProductCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

			//delete categories
			foreach (var existingProductCategory in existingProductCategories)
				if (!selectedCategoryIds.Contains(existingProductCategory.CategoryId))
					await _categoryService.DeleteProductCategoryAsync(existingProductCategory);

			//add categories
			foreach (var categoryId in selectedCategoryIds)
			{
				if (_categoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
				{
					//find next display order
					var displayOrder = 1;
					var existingCategoryMapping = await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
					if (existingCategoryMapping.Any())
						displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
					await _categoryService.InsertProductCategoryAsync(new ProductCategory
					{
						ProductId = product.Id,
						CategoryId = categoryId,
						DisplayOrder = displayOrder
					});
				}
			}
		}

		#endregion
	}
}
