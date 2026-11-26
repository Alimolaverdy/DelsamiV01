using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Services
{
    public class DelsamiUrlRecordService : IDelsamiUrlRecordService
    {
        #region Fields

        private readonly IRepository<UrlRecord> _urlRecordRepo;
        private readonly IRepository<Category> _categoryRepo;

        #endregion

        #region Ctor

        public DelsamiUrlRecordService(IRepository<UrlRecord> urlRecordRepo,
                                        IRepository<Category> categoryRepo)
        {
            _urlRecordRepo = urlRecordRepo;
            _categoryRepo = categoryRepo;
        }

        #endregion

        #region  Methods

        public async Task<List<UrlRecord>> GetSuggestionForCanonicalCategoriesAsync(string categoryName, string categorySeName, int currentEntityId)
        {
            categorySeName = Regex.Replace(categorySeName, @"-\d+", "");

            var parts = categorySeName.Split('-', StringSplitOptions.RemoveEmptyEntries);

            var query = _urlRecordRepo.Table.Where(w => (w.Slug == categorySeName || w.Slug.StartsWith(categorySeName + "-")) && w.EntityName == nameof(Category) && w.EntityId != currentEntityId);

            var categories = _categoryRepo.Table.Where(w => w.Name.Contains(categoryName)).AsQueryable();
            var urlRecords = _urlRecordRepo.Table.Where(w => w.EntityName == nameof(Category)).AsQueryable().Join(categories, u => u.EntityId, c => c.Id, (u, c) => u);

            var allUrlRecordsQuery = query.Union(urlRecords);
            //var urlRecordsBasedOn
            return await allUrlRecordsQuery.ToListAsync();
        }

        #endregion
    }
}
