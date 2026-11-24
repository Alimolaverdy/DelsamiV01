using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Core.Security;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    internal class ZarIranRecentlyViewedProductsService : RecentlyViewedProductsService, IRecentlyViewedProductsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CookieSettings _cookieSettings;
        private readonly ILogger _logger;

        public ZarIranRecentlyViewedProductsService(CatalogSettings catalogSettings,
                                                     CookieSettings cookieSettings,
                                                     IHttpContextAccessor httpContextAccessor,
                                                     IProductService productService,
                                                     IWebHelper webHelper,
                                                     ILogger logger) : base(catalogSettings, cookieSettings, httpContextAccessor, productService, webHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _cookieSettings = cookieSettings;
            _logger = logger;
        }

        protected override Task AddRecentlyViewedProductsCookieAsync(IEnumerable<int> recentlyViewedProductIds)
        {
            _logger.InformationAsync("AddRecentlyViewedProductsCookieAsync");
            _logger.InformationAsync($"{string.Join(",", recentlyViewedProductIds)}");
            //delete current cookie if exists
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.RecentlyViewedProductsCookie}";
            //_httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //create cookie value
            var productIdsCookie = string.Join(",", recentlyViewedProductIds);

            //create cookie options 
            var cookieExpires = _cookieSettings.RecentlyViewedProductsCookieExpires;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(cookieExpires),
                SameSite = SameSiteMode.None,
                Secure = true,
                Domain = "ZarIran.com",
                Path = "/"
            };

            //add cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, productIdsCookie, cookieOptions);

            return Task.CompletedTask;
        }
    }
}
