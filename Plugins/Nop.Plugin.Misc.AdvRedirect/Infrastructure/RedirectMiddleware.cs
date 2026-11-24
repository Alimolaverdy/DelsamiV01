//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Nop.Plugin.Misc.AdvRedirect.Services;
//using Nop.Services.Logging;
//using System;
//using System.Threading.Tasks;
//using System.Web;

//namespace Nop.Plugin.Misc.AdvRedirect.Infrastructure
//{
//    public static class CustomExtensions
//    {
//        public static void UseCustomRedirect(this IApplicationBuilder application)
//        {
//            application.UseMiddleware<RedirectMiddleware>();
//        }
//    }

//    public class RedirectMiddleware
//    {
//        #region Fields

//        private readonly IRedirectionsService _redirectionService;
//        private readonly RequestDelegate _next;
//        private readonly ILogger _logger;

//        #endregion

//        #region Ctor

//        public RedirectMiddleware(IAuthenticationSchemeProvider schemes, ILogger logger, IRedirectionsService redirectionService, RequestDelegate next)
//        {
//            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
//            _next = next ?? throw new ArgumentNullException(nameof(next));
//            _redirectionService = redirectionService;
//            _logger = logger;
//        }

//        #endregion

//        #region Properties

//        /// <summary>
//        /// 
//        /// </summary>
//        public IAuthenticationSchemeProvider Schemes { get; set; }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// Invoke middleware actions
//        /// </summary>
//        /// <param name="context">HTTP context</param>
//        /// <returns>A task that represents the asynchronous operation</returns>
//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                if (context.Request.Method == "GET")
//                {
//                    string redirectUrl = await _redirectionService.ResolveRedirection(context.Request);
//                    if (!string.IsNullOrEmpty(redirectUrl))
//                    {
//                        var isApiRequest = context.Request.Path.StartsWithSegments("/api") ||
//                                          context.Request.Path.StartsWithSegments("/api-frontend") ||
//                                          context.Request.Headers["Accept"].ToString().Contains("application/json");

//                        if (!context.Response.HasStarted)
//                        {
//                            if (isApiRequest)
//                            {
//                                var encodedUrl = HttpUtility.UrlPathEncode(redirectUrl);
//                                context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
//                                context.Response.Headers.Location = encodedUrl;
//                                return;
//                            }
//                            else
//                            {
//                                var parsed = HttpUtility.UrlEncode(redirectUrl).Replace("%2f", "/");
//                                context.Response.Redirect(parsed, true);
//                                return;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                await _logger.ErrorAsync("AdvRedirect", ex);
//            }

//            await _next(context);
//        }

//        #endregion
//    }
//}


