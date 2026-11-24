using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Misc.AdvRedirect.Rules;

public class RegexRule : IRule, IRuleWithType
{
    private Regex _regex;
    private string _redirectUrl;
    private bool _useQryString;
    private RedirectionTypeEnum _redirectionType;

    //public RegexRule(string pattern, string redirectUrl, bool useQryString)
    //{
    //    _redirectUrl = redirectUrl;
    //    _regex = new Regex(pattern);
    //    _useQryString = useQryString;
    //}
    public RegexRule(string pattern, string redirectUrl, bool useQryString, RedirectionTypeEnum redirectionType)
    {
        _redirectUrl = redirectUrl;
        _regex = new Regex(pattern);
        _useQryString = useQryString;
        _redirectionType = redirectionType;
    }

    public bool UseQueryString
    {
        get
        {
            return _useQryString;
        }
    }

    public RedirectionTypeEnum RedirectType
    {
        get
        {
            return _redirectionType;
        }
    }


    public string RedirectUrl
    {
        get
        {
            return _redirectUrl;
        }
    }

    public Regex Regex
    {
        get
        {
            return _regex;
        }
    }


    public bool Match(string url, string qry)
    {
        string key = _useQryString ? url + qry : url;
        return _regex.Match(key).Success;
    }
}
