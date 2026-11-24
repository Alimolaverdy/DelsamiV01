using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Enums;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public interface IRedirectionsService
    {
        Task DeleteRedirectionAsync(RedirectionRule ent);

        Task<IPagedList<RedirectionRule>> GetAllRedirectionsAsync(RedirectionSearchModel searchModel);


        Task<InsertRedirectionResult> InsertRedirectionsAsync(RedirectionRule ent);

        Task<string> ResolveRedirection(HttpRequest request);

        Task<string> ResolveRedirectionBySlug(string slug);
    }
}