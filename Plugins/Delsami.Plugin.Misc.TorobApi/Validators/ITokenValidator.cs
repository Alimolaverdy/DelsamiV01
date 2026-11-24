using ZarIran.Plugin.Misc.TorobApi.Models.Validators;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.TorobApi.Validators
{
    public interface ITokenValidator
    {
        Task<TokenValidationResult> ValidateToken(string token);
    }
}
