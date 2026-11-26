using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using Delsami.Plugin.Misc.Core.Events;
using Delsami.Plugin.Misc.ShipmentManagement.Domain;
using Delsami.Plugin.Misc.ShipmentManagement.Models.ShipmentTrackNumberImport;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using OfficeOpenXml;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Services;

public class ShipmentTrackingImportService : IShipmentTrackingImportService
{
    #region Fields

    private readonly IRepository<ShipmentTrackingNumberImport> _importRepository;
    private readonly IRepository<ShipmentTrackingNumberImporterReport> _reportRepository;
    private readonly IShipmentService _shipmentService;
    private readonly IOrderService _orderService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ShipmentManagementSettings _shipmentSettings;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly ILocalizationService _localizationService;
    #endregion

    #region Ctor

    public ShipmentTrackingImportService(
        IRepository<ShipmentTrackingNumberImport> importRepository,
        IRepository<ShipmentTrackingNumberImporterReport> reportRepository,
        IShipmentService shipmentService,
        IOrderService orderService,
        ShipmentManagementSettings shipmentSettings,
        IEventPublisher eventPublisher,
        IOrderProcessingService orderProcessingService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService)
    {
        _importRepository = importRepository;
        _shipmentSettings = shipmentSettings;
        _reportRepository = reportRepository;
        _shipmentService = shipmentService;
        _orderService = orderService;
        _eventPublisher = eventPublisher;
        _orderProcessingService = orderProcessingService;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
    }
    #endregion

    public async Task<int> ImportTrackingNumbersAsync(byte[] fileBytes, int userId)
    {

        var import = new ShipmentTrackingNumberImport
        {
            BinaryData = fileBytes,
            CreatedOnUtc = DateTime.UtcNow
        };
        await _importRepository.InsertAsync(import);

        var reports = new List<ShipmentTrackingNumberImporterReport>();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var memoryStream = new MemoryStream(fileBytes))
        using (var package = new ExcelPackage(memoryStream))
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new NopException("No worksheet found");

            int tracknumberColIndex = -1;
            int shipmentIdColIndex = -1;

            string shipmentWord = _shipmentSettings.ShipmentIdIndicatorWordInFile ?? throw new NopException("ShipmentIdIndicatorWordInFile property is null");
            ;
            string trackingNumberWord = _shipmentSettings.TrackingNumberIndicatorWordInFile ?? throw new NopException("TrackingNumberIndicatorWordInFile property is null");

            for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
            {
                if (shipmentIdColIndex != -1 && tracknumberColIndex != -1) break;

                if (worksheet.Cells[1, col].Text.Trim().Contains(shipmentWord))
                    shipmentIdColIndex = col;


                if (worksheet.Cells[1, col].Text.Trim().Contains(trackingNumberWord))
                    tracknumberColIndex = col;

            }

            if (shipmentIdColIndex == -1 && tracknumberColIndex == -1)
                throw new NopException("File Is Not Valid");


            int rowCount = worksheet.Dimension?.Rows ?? 0;
            for (int row = 2; row <= rowCount; row++)
            {

                var shipmentNumber = worksheet.Cells[row, shipmentIdColIndex].Value?.ToString();
                var trackingNumber = worksheet.Cells[row, tracknumberColIndex].Value?.ToString()?.Trim();

                if (string.IsNullOrEmpty(shipmentNumber) && string.IsNullOrEmpty(trackingNumber)) continue;

                if (!GetNumber(shipmentNumber, out int shipmentId))
                {
                    reports.Add(CreateReport(import.Id, shipmentId, 0, trackingNumber, ImportTrackNumberResult.ShipmentIdIsNotValid, $"{shipmentNumber}  :شماره مرسوله نامعتبر", userId));
                    continue;
                }

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
                if (shipment == null)
                {
                    reports.Add(CreateReport(import.Id, shipmentId, 0, trackingNumber, ImportTrackNumberResult.ShipmentNotFound, $"{shipmentId}  :شماره مرسوله یافت نشد", userId));
                    continue;
                }

                var existingTracking = shipment.TrackingNumber?.Trim();
                if (!string.IsNullOrEmpty(existingTracking))
                {
                    if (existingTracking == trackingNumber)
                    {
                        reports.Add(CreateReport(import.Id, shipmentId, shipment.OrderId, trackingNumber, ImportTrackNumberResult.TrackingNumberIsTheSame, $"قدیمی: {existingTracking}, جدید: {trackingNumber}", userId));
                        continue;
                    }
                    else
                    {
                        reports.Add(CreateReport(import.Id, shipmentId, shipment.OrderId, trackingNumber, ImportTrackNumberResult.TrackingNumberIsNotTheSame, $"قدیمی: {existingTracking}, جدید: {trackingNumber}", userId));
                        continue;
                    }
                }

                shipment.TrackingNumber = trackingNumber;

                if (!shipment.ShippedDateUtc.HasValue)
                {
                    await _orderProcessingService.ShipAsync(shipment, true);
                    await LogEditOrderAsync(shipment.OrderId);
                }

                await _shipmentService.UpdateShipmentAsync(shipment);

                bool sendSms = false;
                if (string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                    sendSms = true;

                var trackingNumberSetEvent = new TrackingNumberSetEvent
                {
                    Shipment = shipment,
                    SendSMS = sendSms,
                };

                await _eventPublisher.PublishAsync(trackingNumberSetEvent);

                reports.Add(CreateReport(import.Id, shipmentId, shipment.OrderId, trackingNumber, ImportTrackNumberResult.Success, null, userId));
            }
        }

        if (reports.Any())
            await _reportRepository.InsertAsync(reports);

        return import.Id;
    }

    private ShipmentTrackingNumberImporterReport CreateReport(int importId, int shipmentNumber, int orderId, string TrackingNumber, ImportTrackNumberResult status, string errorDesc, int userId)
    {
        return new ShipmentTrackingNumberImporterReport
        {
            ShipmentTrackingNumberImportId = importId,
            ShipmentId = shipmentNumber,
            OrderId = orderId,
            TrackingNumber = TrackingNumber,
            Status = (int)status,
            ErrorDescription = errorDesc,
            CreatedOnUtc = DateTime.UtcNow,
            CustomerId = userId
        };
    }

    public async Task<IList<ShipmentTrackingNumberImporterReport>> GetReportsByImportIdAsync(int importId)
    {
        return await _reportRepository.Table.Where(r => r.ShipmentTrackingNumberImportId == importId).ToListAsync();
    }

    public async Task<IPagedList<ShipmentTrackingNumberImport>> GetImportsAsync(ImportSearchModel searchModel)
    {
        var query = _importRepository.Table;
        if (searchModel.StartDate.HasValue)
            query = query.Where(i => i.CreatedOnUtc.Date >= searchModel.StartDate.Value.Date);
        if (searchModel.EndDate.HasValue)
            query = query.Where(i => i.CreatedOnUtc.Date <= searchModel.EndDate.Value.Date);
        var result = await query.Select(a => new ShipmentTrackingNumberImport()
        {
            Id = a.Id,
            CreatedOnUtc = a.CreatedOnUtc

        }).OrderByDescending(i => i.CreatedOnUtc).ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

        return result;
    }

    public async Task<IList<ShipmentTrackingNumberImporterReport>> GetAllReportsAsync()
    {
        return await _reportRepository.Table.ToListAsync();
    }

    public async Task<IPagedList<ShipmentTrackingNumberImporterReport>> GetReportsByImportIdWithFilterAsync(ReportSearchModel reportSearchModel)
    {
        var query = _reportRepository.Table.Where(r => r.ShipmentTrackingNumberImportId == reportSearchModel.ShipmentImportId);
        if (reportSearchModel.Status != 0)
            query = query.Where(r => r.Status == reportSearchModel.Status);

        query = query.OrderByDescending(r => r.CreatedOnUtc);
        return await query.ToPagedListAsync(reportSearchModel.Page - 1, reportSearchModel.PageSize);
    }

    private bool GetNumber(string input, out int num)
    {
        num = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;

        if (!int.TryParse(input.Trim().Where(char.IsDigit).ToArray(), out int number)) return false;


        num = number;
        return true;
    }

    protected virtual async Task LogEditOrderAsync(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        await _customerActivityService.InsertActivityAsync("EditOrder",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
    }

}
