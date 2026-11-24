using Nop.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;
using Delsami.Plugin.Misc.Core.Enums;

namespace Delsami.Plugin.Misc.Core.Services
{
    public interface IImportReportService
    {
        Task InsertAsync(Stream stream, ImportTypes type);

        Task<IPagedList<ImportReport>> GetImportReportAsync(DateTime? startDate = null,
                                                            DateTime? endDate = null,
                                                            int ImportTypeId = 0,
                                                            int pageIndex = 0,
                                                            int pageSize = int.MaxValue);
        Task<ImportReport> GetByIdAsync(int id);

    }
}
