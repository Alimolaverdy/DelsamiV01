using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace ZarIran.Plugin.Misc.TorobApi.Infrastructure
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }
}
