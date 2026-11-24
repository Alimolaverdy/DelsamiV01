using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;
using Delsami.Plugin.Misc.Core.Enums;

namespace Delsami.Plugin.Misc.Core.Services
{
    public class ImportReportService : IImportReportService
    {
        private readonly IRepository<ImportReport> _repository;
        private readonly IWorkContext _workContext;

        public ImportReportService(IRepository<ImportReport> repository, IWorkContext workContext)
        {
            _repository = repository;
            _workContext = workContext;
        }

        public async Task InsertAsync(Stream stream, ImportTypes type)
        {

            var customer = _workContext.OriginalCustomerIfImpersonated;
            if (customer is null)
                customer = await _workContext.GetCurrentCustomerAsync();

            StreamReader streamReader = new StreamReader(stream);

            var entity = new ImportReport()
            {
                CreatedOn = DateTime.Now,
                CustomerId = customer.Id,
                ImportType = type,
                OrginalFile = ToByteArray(stream)
            };
            await _repository.InsertAsync(entity);

        }


        public async Task<IPagedList<ImportReport>> GetImportReportAsync(DateTime? startDate = null,
                                                                   DateTime? endDate = null,
                                                                   int ImportTypeId = 0,
                                                                   int pageIndex = 0,
                                                                   int pageSize = int.MaxValue)
        {

            var list = await _repository.GetAllPagedAsync(query =>
            {
                if (startDate.HasValue)
                    query = query.Where(w => w.CreatedOn.Date >= startDate.Value.Date);

                if (endDate.HasValue)
                    query = query.Where(w => w.CreatedOn.Date <= endDate.Value.Date);

                if (ImportTypeId > 0)
                    query = query.Where(w => w.ImportType == (ImportTypes)ImportTypeId);

                return query.OrderByDescending(c => c.Id);

            }, pageIndex, pageSize);

            return list;
        }


        public async Task<ImportReport> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }


        private static byte[] ToByteArray(Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            if (stream is MemoryStream ms)
                return ms.ToArray();

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

    }
}
