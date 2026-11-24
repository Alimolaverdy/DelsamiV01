using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Controllers;

public class CustomProductController : ProductController
{

    #region Fields
    private readonly CatalogSettings _catalogSettings;
    private readonly IAclService _aclService;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPermissionService _permissionService;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;
    private readonly ShoppingCartSettings _shoppingCartSettings;
    #endregion

    #region Ctor

    public CustomProductController(CaptchaSettings captchaSettings,
                                   CatalogSettings catalogSettings,
                                   IAclService aclService,
                                   ICompareProductsService compareProductsService,
                                   ICustomerActivityService customerActivityService,
                                   ICustomerService customerService,
                                   IEventPublisher eventPublisher,
                                   IHtmlFormatter htmlFormatter,
                                   ILocalizationService localizationService,
                                   INopUrlHelper nopUrlHelper,
                                   IOrderService orderService,
                                   IPermissionService permissionService,
                                   IProductAttributeParser productAttributeParser,
                                   IProductModelFactory productModelFactory,
                                   IProductService productService,
                                   IRecentlyViewedProductsService recentlyViewedProductsService,
                                   IReviewTypeService reviewTypeService,
                                   IShoppingCartModelFactory shoppingCartModelFactory,
                                   IShoppingCartService shoppingCartService,
                                   IStoreContext storeContext,
                                   IStoreMappingService storeMappingService,
                                   IUrlRecordService urlRecordService,
                                   IWorkContext workContext,
                                   IWorkflowMessageService workflowMessageService,
                                   LocalizationSettings localizationSettings,
                                   ShoppingCartSettings shoppingCartSettings,
                                   ShippingSettings shippingSettings) : base(captchaSettings,
                                                                             catalogSettings,
                                                                             aclService,
                                                                             compareProductsService,
                                                                             customerActivityService,
                                                                             customerService,
                                                                             eventPublisher,
                                                                             htmlFormatter,
                                                                             localizationService,
                                                                             nopUrlHelper,
                                                                             orderService,
                                                                             permissionService,
                                                                             productAttributeParser,
                                                                             productModelFactory,
                                                                             productService,
                                                                             recentlyViewedProductsService,
                                                                             reviewTypeService,
                                                                             shoppingCartModelFactory,
                                                                             shoppingCartService,
                                                                             storeContext,
                                                                             storeMappingService,
                                                                             urlRecordService,
                                                                             workContext,
                                                                             workflowMessageService,
                                                                             localizationSettings,
                                                                             shoppingCartSettings,
                                                                             shippingSettings)


    {

        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _recentlyViewedProductsService = recentlyViewedProductsService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _shoppingCartSettings = shoppingCartSettings;

    }

    #endregion

    #region Methods


    public override async Task<IActionResult> ProductDetails(int productId, int updatecartitemid = 0)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted || product.ProductType == ProductType.SimpleProduct)
            return InvokeHttp404();

        var notAvailable =
            //published?
            (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
        //ACL (access control list) 
            !await _aclService.AuthorizeAsync(product) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(product) ||
            //availability dates
            !_productService.ProductIsAvailable(product);
        //Check whether the current user has a "Manage products" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        //visible individually?
        if (!product.VisibleIndividually)
        {
            //is this one an associated products?
            var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
            if (parentGroupedProduct == null)
                return RedirectToRoute("Homepage");

            var seName = await _urlRecordService.GetSeNameAsync(parentGroupedProduct);
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName });
            return LocalRedirectPermanent(productUrl);
        }

        //update existing shopping cart or wishlist  item?
        ShoppingCartItem updatecartitem = null;
        if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
        {
            var seName = await _urlRecordService.GetSeNameAsync(product);
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName });
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: store.Id);
            updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);

            //not found?
            if (updatecartitem == null)
                return LocalRedirect(productUrl);

            //is it this product?
            if (product.Id != updatecartitem.ProductId)
                return LocalRedirect(productUrl);
        }

        //save as recently viewed
        await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(product.Id);

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) &&
            await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
        {
            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor == null || currentVendor.Id == product.VendorId)
            {
                DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
            }
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("PublicStore.ViewProduct",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

        //model
        var model = await _productModelFactory.PrepareProductDetailsModelAsync(product, updatecartitem, false);
        //template
        var productTemplateViewPath = await _productModelFactory.PrepareProductTemplateViewPathAsync(product);

        return View($"~/Plugins/ZarIran.Misc.ProductManagements/Themes/Traction/Views/Product/ProductTemplate.Grouped.cshtml", model);
    }

    #endregion

}
