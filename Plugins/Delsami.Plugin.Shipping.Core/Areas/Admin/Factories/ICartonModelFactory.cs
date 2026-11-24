using System.Threading.Tasks;
using ZarIran.Plugin.Shipping.Core.Areas.Admin.Models.Cartons;
using ZarIran.Plugin.Shipping.Core.Domain;

namespace ZarIran.Plugin.Shipping.Core.Areas.Admin.Factories
{
    public interface ICartonModelFactory
    {
        CartonSearchModel PrepareCartonSearchModel(CartonSearchModel searchModel);

        CartonModel PrepareCartonModel(CartonModel model, Carton carton);

        Task<CartonListModel> PrepareCartonListModelAsync(CartonSearchModel searchModel);
    }
}
