using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace Delsami.Plugin.Misc.TorobApi.Infrastructure
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }
}
