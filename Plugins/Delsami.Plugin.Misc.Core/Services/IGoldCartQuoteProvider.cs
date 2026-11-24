using System.Threading.Tasks;
namespace Delsami.Plugin.Misc.Core.Services;
/// <summary>Preserves a wallet purchase amount in the cart; null means use normal product pricing.</summary>
public interface IGoldCartQuoteProvider
{
    Task<decimal?> GetUnitPriceAsync(int productId, int customerId, decimal enteredAmount);
}
