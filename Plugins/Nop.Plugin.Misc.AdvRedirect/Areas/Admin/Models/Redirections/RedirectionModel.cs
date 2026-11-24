using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections;

public record RedirectionModel : BaseNopEntityModel
{
    [Required]
    [DataType(DataType.Url)]
    public string Pattern { get; set; }

    public bool UseQueryString { get; set; }


    [DataType(DataType.Url)]
    public string RedirectUrl { get; set; }


    [Required]
    public RedirectionTypeEnum Type { get; set; }


}
