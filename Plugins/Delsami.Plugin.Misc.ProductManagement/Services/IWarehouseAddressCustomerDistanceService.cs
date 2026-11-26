using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public interface IWarehouseAddressCustomerDistanceService
    {
        #region Signatures

        Task InitializeWarehouseAddressCustomerDistancesAsync();

        Task DeleteWarehouseAddressCustomerDistanceByCustomerAddressIdAsync(int customerAddressId);

        Task InsertOrUpdateByAddressIdAsync(int addressId, bool? isCustomerAddress = null, bool? isWarehouseAddress = null);

        Task DeleteWarehouseAddressCustomerDistanceByWarehouseAddressIdAsync(int warehouseAddressId);

        Task<int?> GetDistanceAsync(int customerId, int warehouseId);

        #endregion
    }
}
