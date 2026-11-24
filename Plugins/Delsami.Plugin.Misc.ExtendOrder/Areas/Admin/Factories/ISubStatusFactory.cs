using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories
{
    public interface ISubStatusFactory
    {
        OrderSubStatusSearchModel PrepareOrderSubStatusSearchModel(OrderSubStatusSearchModel searchModel);
        Task<OrderSubStatusListModel> PrepareOrderSubStatusListModelAsync(OrderSubStatusSearchModel searchModel);

        OrderSubStatusModel PrepareOrderSubStatusModel(OrderSubStatusModel model, OrderSubStatus smsReceiver);


    }
}
