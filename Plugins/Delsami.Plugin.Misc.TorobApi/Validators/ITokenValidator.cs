using Delsami.Plugin.Misc.TorobApi.Models.Validators;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.TorobApi.Validators
{
    public interface ITokenValidator
    {
        Task<TokenValidationResult> ValidateToken(string token);
    }
}
