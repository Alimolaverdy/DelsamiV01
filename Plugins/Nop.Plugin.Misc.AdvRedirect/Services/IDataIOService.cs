using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public interface IDataIOService
    {
        Task<List<(int line, InsertRedirectionResult result)>> ImportAsync(List<(int line, RedirectionRule record)> records);


        Task<byte[]> ExportAsync();
	}
}