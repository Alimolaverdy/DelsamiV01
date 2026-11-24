using Nop.Plugin.Misc.AdvRedirect.Domain;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public interface IAdvRedirectHistoryService
    {
        Task InsertAsync(AdvRedirectHistory advRedirectHistory);
    }
}
