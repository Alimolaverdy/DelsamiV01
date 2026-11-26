using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
	public class DelsamiProductService : ProductService, IProductService
	{
		#region Fields

		private readonly IVirtualProductMappingService _virtualProductMappingService;

		#endregion

		#region Ctor

		public DelsamiProductService(CatalogSettings catalogSettings,
									  CommonSettings commonSettings,
									  IAclService aclService,
									  ICustomerService customerService,
									  IDateRangeService dateRangeService,
									  ILanguageService languageService,
									  ILocalizationService localizationService,
									  IProductAttributeParser productAttributeParser,
									  IProductAttributeService productAttributeService,
									  IRepository<Category> categoryRepository,
									  IRepository<CrossSellProduct> crossSellProductRepository,
									  IRepository<DiscountProductMapping> discountProductMappingRepository,
									  IRepository<LocalizedProperty> localizedPropertyRepository,
									  IRepository<Manufacturer> manufacturerRepository,
									  IRepository<Product> productRepository,
									  IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
									  IRepository<ProductAttributeMapping> productAttributeMappingRepository,
									  IRepository<ProductCategory> productCategoryRepository,
									  IRepository<ProductManufacturer> productManufacturerRepository,
									  IRepository<ProductPicture> productPictureRepository,
									  IRepository<ProductProductTagMapping> productTagMappingRepository,
									  IRepository<ProductReview> productReviewRepository,
									  IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
									  IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
									  IRepository<ProductTag> productTagRepository,
									  IRepository<ProductVideo> productVideoRepository,
									  IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
									  IRepository<RelatedProduct> relatedProductRepository,
									  IRepository<Shipment> shipmentRepository,
									  IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
									  IRepository<TierPrice> tierPriceRepository,
									  ISearchPluginManager searchPluginManager,
									  IStaticCacheManager staticCacheManager,
									  IStoreService storeService,
									  IStoreMappingService storeMappingService,
									  IWorkContext workContext,
									  LocalizationSettings localizationSettings,
									  IVirtualProductMappingService virtualProductMappingService) : base(catalogSettings,
																						commonSettings,
																						aclService,
																						customerService,
																						dateRangeService,
																						languageService,
																						localizationService,
																						productAttributeParser,
																						productAttributeService,
																						categoryRepository,
																						crossSellProductRepository,
																						discountProductMappingRepository,
																						localizedPropertyRepository,
																						manufacturerRepository,
																						productRepository,
																						productAttributeCombinationRepository,
																						productAttributeMappingRepository,
																						productCategoryRepository,
																						productManufacturerRepository,
																						productPictureRepository,
																						productTagMappingRepository,
																						productReviewRepository,
																						productReviewHelpfulnessRepository,
																						productSpecificationAttributeRepository,
																						productTagRepository,
																						productVideoRepository,
																						productWarehouseInventoryRepository,
																						relatedProductRepository,
																						shipmentRepository,
																						stockQuantityHistoryRepository,
																						tierPriceRepository,
																						searchPluginManager,
																						staticCacheManager,
																						storeService,
																						storeMappingService,
																						workContext,
																						localizationSettings)
		{
			_virtualProductMappingService = virtualProductMappingService;
		}

		#endregion

		#region Methods

		public override async Task<int> ReverseBookedInventoryAsync(Product product, ShipmentItem shipmentItem, string message = "")
		{
			if (product == null)
				throw new ArgumentNullException(nameof(product));

			Product mainProduct = null;

			//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
			//DeliveryDateId is 'MainProductId'
			if (product.AllowAddingOnlyExistingAttributeCombinations)
				mainProduct = await GetProductByIdAsync(product.DeliveryDateId);

			if (shipmentItem == null)
				throw new ArgumentNullException(nameof(shipmentItem));

			if (mainProduct is null)
				throw new NullReferenceException(nameof(mainProduct));

			if (mainProduct != null)
				return await ReverseAsync(mainProduct, shipmentItem, message);
			else
				return await ReverseAsync(product, shipmentItem, message);

			////only products with "use multiple warehouses" are handled this way
			//if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
			//    return 0;

			//var pwi = await _productWarehouseInventoryRepository.Table.FirstOrDefaultAsync(wi => wi.ProductId == product.Id && wi.WarehouseId == shipmentItem.WarehouseId);
			//if (pwi == null)
			//    return 0;

			//var shipment = await _shipmentRepository.GetByIdAsync(shipmentItem.ShipmentId, cache => default);

			////not shipped yet? hence "BookReservedInventory" method was not invoked
			//if (!shipment.ShippedDateUtc.HasValue)
			//    return 0;

			//var qty = shipmentItem.Quantity;

			//pwi.StockQuantity += qty;
			//pwi.ReservedQuantity += qty;

			//await UpdateProductWarehouseInventoryAsync(pwi);

			////quantity change history
			//await AddStockQuantityHistoryEntryAsync(product, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

			//return qty;
		}

		public override async Task AdjustInventoryAsync(Product product, int quantityToChange, string attributesXml = "", string message = "")
		{
			Product mainProduct = null;

			//AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
			//DeliveryDateId is 'MainProductId'
			if (product.AllowAddingOnlyExistingAttributeCombinations)
				mainProduct = await GetProductByIdAsync(product.DeliveryDateId);

			await AdjustInventoryInternalAsync(mainProduct ?? product, quantityToChange, attributesXml, message);
		}

		protected override async Task<string> GetStockMessageAsync(Product product)
		{
			var reservedStockQuantityService = EngineContext.Current.Resolve<IReservedStockQuantityService>();

			if (!product.DisplayStockAvailability)
				return string.Empty;

			var stockMessage = string.Empty;
			var stockQuantity = await GetTotalStockQuantityAsync(product) - (await reservedStockQuantityService.GetReservedStockQuantityCountAsync(product.Id));

			if (stockQuantity > 0)
			{
				if (product.MinStockQuantity >= stockQuantity && product.LowStockActivity == LowStockActivity.Nothing)
				{
					stockMessage = product.DisplayStockQuantity
						?
						//display "low stock" with stock quantity
						string.Format(await _localizationService.GetResourceAsync("Products.Availability.LowStockWithQuantity"), stockQuantity)
						:
						//display "low stock" without stock quantity
						await _localizationService.GetResourceAsync("Products.Availability.LowStock");
				}
				else
				{
					stockMessage = product.DisplayStockQuantity
						?
						//display "in stock" with stock quantity
						string.Format(await _localizationService.GetResourceAsync("Products.Availability.InStockWithQuantity"), stockQuantity)
						:
						//display "in stock" without stock quantity
						await _localizationService.GetResourceAsync("Products.Availability.InStock");
				}
			}
			else
			{
				//out of stock
				var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
				switch (product.BackorderMode)
				{
					case BackorderMode.NoBackorders:
						stockMessage = productAvailabilityRange == null
							? await _localizationService.GetResourceAsync("Products.Availability.OutOfStock")
							: string.Format(await _localizationService.GetResourceAsync("Products.Availability.AvailabilityRange"),
								await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
						break;
					case BackorderMode.AllowQtyBelow0:
						stockMessage = await _localizationService.GetResourceAsync("Products.Availability.InStock");
						break;
					case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
						stockMessage = productAvailabilityRange == null
							? await _localizationService.GetResourceAsync("Products.Availability.Backordering")
							: string.Format(await _localizationService.GetResourceAsync("Products.Availability.BackorderingWithDate"),
								await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
						break;
				}
			}

			return stockMessage;
		}

		public override async Task<IList<Product>> GetAssociatedProductsAsync(int parentGroupedProductId, int storeId = 0, int vendorId = 0, bool showHidden = false)
		{
			var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
							new CacheKey("DelsamiAssociatedProducts-{0}-{1}-{2}-{3}", "DelsamiAssociatedProducts-{0}"),
							parentGroupedProductId,
							storeId,
							vendorId,
							showHidden);

			return await _staticCacheManager.GetAsync(cacheKey, () => base.GetAssociatedProductsAsync(parentGroupedProductId, storeId, vendorId, showHidden));

		}

		//public override async Task<IList<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId, bool showHidden = false)
		//{

		//	var relatedProducts = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.RelatedProductsCacheKey, productId, showHidden),
		//						  async () =>
		//						   {
		//							   var query = from rp in _relatedProductRepository.Table
		//										   join p in _productRepository.Table on rp.ProductId2 equals p.Id
		//										   where (rp.ProductId1 == productId || rp.ProductId2 == productId) &&
		//										   !p.Deleted &&
		//										   (showHidden || p.Published)
		//										   orderby rp.DisplayOrder, rp.Id
		//										   select rp;
		//							   return await query.ToListAsync();
		//						   });
		

  //          return relatedProducts;
  //      }

	#endregion

	#region Utilities

	private async Task<int> ReverseAsync(Product product, ShipmentItem shipmentItem, string message = "")
	{
		//only products with "use multiple warehouses" are handled this way
		if (product.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !product.UseMultipleWarehouses)
			return 0;

		var pwi = await _productWarehouseInventoryRepository.Table.FirstOrDefaultAsync(wi => wi.ProductId == product.Id && wi.WarehouseId == shipmentItem.WarehouseId);
		if (pwi == null)
			return 0;

		var shipment = await _shipmentRepository.GetByIdAsync(shipmentItem.ShipmentId, cache => default);

		//not shipped yet? hence "BookReservedInventory" method was not invoked
		if (!shipment.ShippedDateUtc.HasValue)
			return 0;

		var qty = shipmentItem.Quantity;

		pwi.StockQuantity += qty;
		pwi.ReservedQuantity += qty;

		await UpdateProductWarehouseInventoryAsync(pwi);

		//quantity change history
		await AddStockQuantityHistoryEntryAsync(product, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

		return qty;
	}

	public async Task AdjustInventoryInternalAsync(Product product, int quantityToChange, string attributesXml = "", string message = "")
	{
		if (product == null)
			throw new ArgumentNullException(nameof(product));

		if (quantityToChange == 0)
			return;

		if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
		{
			//update stock quantity
			if (product.UseMultipleWarehouses)
			{
				//use multiple warehouses
				if (quantityToChange < 0)
					await ReserveInventoryAsync(product, quantityToChange);
				else
					await UnblockReservedInventoryAsync(product, quantityToChange);
			}
			else
			{
				//do not use multiple warehouses
				//simple inventory management
				product.StockQuantity += quantityToChange;
				await UpdateProductAsync(product);

				//quantity change history
				await AddStockQuantityHistoryEntryAsync(product, quantityToChange, product.StockQuantity, product.WarehouseId, message);
			}

			var totalStock = await GetTotalStockQuantityAsync(product);

			await ApplyLowStockActivityAsync(product, totalStock);

			//send email notification
			if (quantityToChange < 0 && totalStock < product.NotifyAdminForQuantityBelow)
			{
				//do not inject IWorkflowMessageService via constructor because it'll cause circular references
				var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
				await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(product, _localizationSettings.DefaultAdminLanguageId);
			}
		}

		if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
		{
			var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
			if (combination != null)
			{
				combination.StockQuantity += quantityToChange;
				await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);

				//quantity change history
				await AddStockQuantityHistoryEntryAsync(product, quantityToChange, combination.StockQuantity, message: message, combinationId: combination.Id);

				if (product.AllowAddingOnlyExistingAttributeCombinations)
				{
					var totalStockByAllCombinations = await (await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
						.ToAsyncEnumerable()
						.SumAsync(c => c.StockQuantity);

					await ApplyLowStockActivityAsync(product, totalStockByAllCombinations);
				}

				//send email notification
				if (quantityToChange < 0 && combination.StockQuantity < combination.NotifyAdminForQuantityBelow)
				{
					//do not inject IWorkflowMessageService via constructor because it'll cause circular references
					var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
					await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(combination, _localizationSettings.DefaultAdminLanguageId);
				}
			}
		}

		if (quantityToChange != 0 && product.ParentGroupedProductId > 0)
		{
			await UpdateParentStockIncrementalAsync(product.Id, quantityToChange);
		}

		//bundled products
		var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
		foreach (var attributeValue in attributeValues)
		{
			if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
				continue;

			//associated product (bundle)
			var associatedProduct = await GetProductByIdAsync(attributeValue.AssociatedProductId);
			if (associatedProduct != null)
			{
				await AdjustInventoryAsync(associatedProduct, quantityToChange * attributeValue.Quantity, message);
			}
		}
	}

	private async Task UpdateParentStockIncrementalAsync(int childProductId, int quantityDelta)
	{
		if (quantityDelta == 0)
			return;

		var child = await GetProductByIdAsync(childProductId);
		if (child == null)
			return;

		if (child.ParentGroupedProductId == 0)
			return;

		var parent = await GetProductByIdAsync(child.ParentGroupedProductId);
		if (parent == null)
			return;

		parent.StockQuantity += quantityDelta;

		if (parent.StockQuantity < 0)
			parent.StockQuantity = 0;

		await UpdateProductAsync(parent);
	}

	#endregion
}
}
