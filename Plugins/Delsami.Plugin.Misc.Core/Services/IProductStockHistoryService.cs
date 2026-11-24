using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services
{
    public interface IProductStockHistoryService
    {
        Task<int> GetPreviousStockQuantityAsync(Product product);
    }
}
