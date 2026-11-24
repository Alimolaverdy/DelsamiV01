using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models.Redirections
{
    /// <summary>
    /// Represents a tax transaction log list model
    /// </summary>
    public record RedirectionsListModel : BasePagedListModel<RedirectionModel>
    {
    }
}