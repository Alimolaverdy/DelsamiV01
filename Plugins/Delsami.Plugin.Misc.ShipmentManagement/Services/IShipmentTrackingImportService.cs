using Delsami.Plugin.Misc.ShipmentManagement.Domain;
using Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Services;

public interface IShipmentTrackingImportService
{
    Task<int> ImportTrackingNumbersAsync(byte[] fileBytes, int userId);
    Task<IList<ShipmentTrackingNumberImporterReport>> GetReportsByImportIdAsync(int importId);
    Task<IPagedList<ShipmentTrackingNumberImport>> GetImportsAsync(ImportSearchModel searchModel);
    Task<IList<ShipmentTrackingNumberImporterReport>> GetAllReportsAsync();
    Task<IPagedList<ShipmentTrackingNumberImporterReport>> GetReportsByImportIdWithFilterAsync(ReportSearchModel reportSearchModel);
}
