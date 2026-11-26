using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public interface IDelsamiOrderReportService
    {
        Task<DelsamiOrderAverageReportLine> GetOrderAverageReportLineAsync(int storeId = 0,
                                                                    int vendorId = 0,
                                                                    int productId = 0,
                                                                    int warehouseId = 0,
                                                                    int billingCountryId = 0,
                                                                    int orderId = 0,
                                                                    string paymentMethodSystemName = null,
                                                                    List<int> osIds = null,
                                                                    List<int> psIds = null,
                                                                    List<int> ssIds = null,
                                                                    DateTime? startTimeUtc = null,
                                                                    DateTime? endTimeUtc = null,
                                                                    string billingPhone = null,
                                                                    string billingEmail = null,
                                                                    string billingLastName = "",
                                                                    string orderNotes = null);
    }
}
