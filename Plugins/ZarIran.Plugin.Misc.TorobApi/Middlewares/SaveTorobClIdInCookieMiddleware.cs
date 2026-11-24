using Microsoft.AspNetCore.Http;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZarIran.Plugin.Misc.TorobApi.Middlewares
{
    public class SaveTorobClIdInCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public SaveTorobClIdInCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, TorobApiSettings torobApiSettings)
        {

            var request = context.Request;
            var torobClid = request.Query["torob_clid"].FirstOrDefault();

            if (torobApiSettings.IsActive && !string.IsNullOrEmpty(torobClid))
            {
                var cookieName = "torob_clid";

                //var cookieExists = request.Cookies.TryGetValue(cookieName,out var existingTorobClid);

                //var value = cookieExists && !string.IsNullOrEmpty(existingTorobClid)
                //			? existingTorobClid
                //			: torobClid;

                context.Response.Cookies.Append(
                    cookieName,
                    torobClid,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        MaxAge = TimeSpan.FromDays(7),
                        Path = "/"
                    });
            }

            await _next(context);
        }

    }
}
