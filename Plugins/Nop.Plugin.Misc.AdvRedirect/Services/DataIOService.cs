using ClosedXML.Excel;
using CsvHelper.Configuration;
using LinqToDB;
using Nop.Core;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Enums;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public class DataIOService : IDataIOService
    {
        #region Fields

        private readonly IRepository<RedirectionRule> _redirectionRuleEntityRepository;
        private readonly IRedirectionsService _service;
        private readonly IStoreContext _storeContext;
        private readonly CsvConfiguration _csvConf = new CsvConfiguration(CultureInfo.InvariantCulture);

        #endregion

        #region Ctor

        public DataIOService(IRedirectionsService service, IStoreContext storeContext, IRepository<RedirectionRule> redirectionRuleEntityRepository)
        {
            _service = service;
            _storeContext = storeContext;
            _redirectionRuleEntityRepository = redirectionRuleEntityRepository;
            _csvConf.Delimiter = ";";
        }

        #endregion

        #region Methods

        public async Task<byte[]> ExportAsync()
        {

            var store = await _storeContext.GetCurrentStoreAsync();
            var data = await _redirectionRuleEntityRepository.Table.Where(r => r.StoreId == store.Id).Select(r => AutoMapperConfiguration.Mapper.Map<RedirectionCSVModel>(r)).ToListAsync();


            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Export");

            worksheet.Cell(1, 1).InsertTable(data, "Export", true);

            worksheet.ColumnsUsed().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        public async Task<List<(int line, InsertRedirectionResult result)>> ImportAsync(List<(int line,RedirectionRule record)> records)
        {
            var errors = new List<(int line, InsertRedirectionResult result)>();

            foreach (var error in records)
            {
                var result = await _service.InsertRedirectionsAsync(error.record);
                if (result != InsertRedirectionResult.Ok)
                    errors.Add((error.line, result));
            } 

            return errors;
        }

        #endregion
    }
}
