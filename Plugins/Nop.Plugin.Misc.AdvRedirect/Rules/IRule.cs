using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.AdvRedirect.Rules
{
    public interface IRule
    {
        public string RedirectUrl { get; }

        
        public bool Match(string url, string qry);
    } 

    public interface IRuleWithType: IRule
    {
        public RedirectionTypeEnum RedirectType { get; }
        public Regex Regex { get; }
        public bool UseQueryString { get; }


    }
}
