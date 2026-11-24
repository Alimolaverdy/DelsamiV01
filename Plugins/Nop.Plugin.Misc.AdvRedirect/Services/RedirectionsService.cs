using LinqToDB;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Enums;
using Nop.Plugin.Misc.AdvRedirect.Rules;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public class RedirectionsService : IRedirectionsService
    {
        private readonly IRepository<RedirectionRule> _redirectionRuleEntityRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;

        public RedirectionsService(
            IRepository<RedirectionRule> redirectionRuleEntityRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ILogger logger)
        {
            _redirectionRuleEntityRepository = redirectionRuleEntityRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _logger = logger;
        }

        private async Task<(Dictionary<string, string> mach, Dictionary<string, string> qryMach, IEnumerable<IRuleWithType> regex)> GetAllRulesAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            CacheKey key = _staticCacheManager.PrepareKeyForDefaultCache(AdvRedirectDefaults.RedirectionsRulesCacheKey, store);

            // نکته مهم: try/catch را از داخل کش حذف کردیم تا در صورت خطا،
            // نتیجه ناقص در کش ذخیره نشود و درخواست بعدی دوباره تلاش کند.
            return await _staticCacheManager.GetAsync(key, async () =>
            {
                // تنها یک بار داده از دیتابیس خوانده می‌شود
                var data = await _redirectionRuleEntityRepository.Table
                    .Where(r => r.StoreId == store.Id)
                    .ToListAsync();

                var mach = data
                    .Where(r => (RedirectionTypeEnum)r.Type == RedirectionTypeEnum.Match && !r.UseQueryString)
                    .ToDictionary(r => r.Pattern, r => r.RedirectUrl);

                var qryMach = data
                    .Where(r => (RedirectionTypeEnum)r.Type == RedirectionTypeEnum.Match && r.UseQueryString)
                    .ToDictionary(r => r.Pattern, r => r.RedirectUrl);

                // چون data در حافظه است، از ToList() معمولی استفاده می‌کنیم نه ToListAsync()
                var regex = data
                    .Where(r => (RedirectionTypeEnum)r.Type != RedirectionTypeEnum.Match)
                    .Select(r => (IRuleWithType)new RegexRule(
                        r.Pattern,
                        r.RedirectUrl,
                        r.UseQueryString,
                        (RedirectionTypeEnum)r.Type))
                    .ToList();

                return (
                    mach,
                    qryMach,
                    (IEnumerable<IRuleWithType>)regex
                );
            });
        }

        public async Task<IPagedList<RedirectionRule>> GetAllRedirectionsAsync(RedirectionSearchModel searchModel)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var qry = _redirectionRuleEntityRepository.Table.Where(r => r.StoreId == store.Id);

            if (!string.IsNullOrEmpty(searchModel.Pattern))
                qry = qry.Where(r => r.Pattern.Contains(searchModel.Pattern));

            if (!string.IsNullOrEmpty(searchModel.RedirectUrl))
                qry = qry.Where(r => r.RedirectUrl.Contains(searchModel.RedirectUrl));

            string colName = searchModel.Columns[searchModel.Order[0].Column].Data;

            if (colName == nameof(RedirectionRule.Pattern))
                qry = searchModel.Order[0].Dir == "asc"
                    ? qry.OrderBy(r => r.Pattern)
                    : qry.OrderByDescending(r => r.Pattern);
            else if (colName == nameof(RedirectionRule.RedirectUrl))
                qry = searchModel.Order[0].Dir == "asc"
                    ? qry.OrderBy(r => r.RedirectUrl)
                    : qry.OrderByDescending(r => r.RedirectUrl);
            else if (colName == nameof(RedirectionRule.Id))
                qry = searchModel.Order[0].Dir == "asc"
                    ? qry.OrderBy(r => r.Id)
                    : qry.OrderByDescending(r => r.Id);
            else if (colName == nameof(RedirectionRule.Type))
                qry = searchModel.Order[0].Dir == "asc"
                    ? qry.OrderBy(r => r.Type)
                    : qry.OrderByDescending(r => r.Type);
            else if (colName == nameof(RedirectionRule.UseQueryString))
                qry = searchModel.Order[0].Dir == "asc"
                    ? qry.OrderBy(r => r.UseQueryString)
                    : qry.OrderByDescending(r => r.UseQueryString);

            var records = new PagedList<RedirectionRule>(
                await qry.ToListAsync(),
                searchModel.Page - 1,
                searchModel.PageSize);

            return records;
        }

        public async Task<string> ResolveRedirection(HttpRequest request)
        {
            var rules = await GetAllRulesAsync();

            var path = HttpUtility.UrlDecode(request.Path);
            var queryString = HttpUtility.UrlDecode(request.QueryString.ToString());
            var pathWithQueryString = HttpUtility.UrlDecode(request.Path + request.QueryString.ToString());

            string redirectUrl;

            if (rules.qryMach != null && rules.qryMach.TryGetValue(pathWithQueryString, out redirectUrl))
                return redirectUrl;

            if (rules.mach != null && rules.mach.TryGetValue(path, out redirectUrl))
                return redirectUrl;

            if (rules.regex != null)
            {
                IRuleWithType rule = rules.regex.FirstOrDefault(r => r.Match(path, queryString));
                if (rule != null)
                {
                    if (rule.RedirectType == RedirectionTypeEnum.RegularExpresion)
                        return rule.RedirectUrl;

                    if (rule.RedirectType == RedirectionTypeEnum.RegexSourceAndDestination)
                        return rule.Regex.Replace(
                            OrginaRoute(path, queryString, rule.UseQueryString),
                            rule.RedirectUrl);
                }
            }

            return null;
        }

        public string OrginaRoute(string url, string qry, bool useQuery)
        {
            return useQuery ? url + qry : url;
        }

        private static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public virtual async Task DeleteRedirectionAsync(RedirectionRule ent)
        {
            var item = await _redirectionRuleEntityRepository.GetByIdAsync(ent.Id);
            if (item != null)
                await _redirectionRuleEntityRepository.DeleteAsync(item);
        }

        public async Task<InsertRedirectionResult> InsertRedirectionsAsync(RedirectionRule ent)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            ent.Pattern = ent.Pattern.Trim();

            switch ((RedirectionTypeEnum)ent.Type)
            {
                case RedirectionTypeEnum.Match:

                    if (_redirectionRuleEntityRepository.Table
                        .Any(r => r.StoreId == store.Id
                                  && r.Pattern == ent.Pattern
                                  && r.Type == (int)RedirectionTypeEnum.Match))
                        return InsertRedirectionResult.Exist;

                    if (!ent.RedirectUrl.StartsWith("/"))
                        ent.RedirectUrl = "/" + ent.RedirectUrl;

                    break;

                case RedirectionTypeEnum.RegularExpresion:
                    if (!IsValidRegex(ent.Pattern))
                        return InsertRedirectionResult.RegularExpressionNotValid;
                    break;
            }

            ent.UseQueryString = ent.Type == (int)RedirectionTypeEnum.Match && ent.Pattern.Contains("?")
                ? true
                : ent.UseQueryString;

            ent.StoreId = store.Id;
            await _redirectionRuleEntityRepository.InsertAsync(ent);

            return InsertRedirectionResult.Ok;
        }

        public async Task<string> ResolveRedirectionBySlug(string slug)
        {
            var rules = await GetAllRulesAsync();

            slug = HttpUtility.UrlDecode(slug);

            string redirectUrl;

            if (rules.mach.TryGetValue(slug, out redirectUrl))
                return redirectUrl;

            var regexRule = rules.regex.FirstOrDefault(r => r.Match(slug, ""));

            if (regexRule != null)
            {
                if (regexRule.RedirectType == RedirectionTypeEnum.RegularExpresion)
                    return regexRule.RedirectUrl;

                if (regexRule.RedirectType == RedirectionTypeEnum.RegexSourceAndDestination)
                    return regexRule.Regex.Replace(slug, regexRule.RedirectUrl);
            }

            return null;
        }
    }
}