using System.Threading.Tasks;
using ZarIran.Plugin.Widgets.AddressLocation.Domain;

namespace ZarIran.Plugin.Widgets.AddressLocation.Services.StateProvinceLocations
{
    public interface IStateProvinceLocationService
    {
        Task<StateProvinceLocation> GetStateProvinceLocationByIdAsync(int stateProvinceId);
    }
}
