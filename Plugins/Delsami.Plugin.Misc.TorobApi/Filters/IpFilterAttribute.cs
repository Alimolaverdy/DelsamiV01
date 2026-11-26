using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Delsami.Plugin.Misc.TorobApi.Models.Filters;
using Nop.Services.Logging;
using StackExchange.Profiling.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Delsami.Plugin.Misc.TorobApi.Filters
{
	public class IpFilterAttribute : IActionFilter
	{
		private readonly List<IPAddressRange> _allowedRanges;
		private readonly ILogger _logger;

		public IpFilterAttribute(ILogger logger)
		{
			_allowedRanges = new List<IPAddressRange>
		{
			new IPAddressRange(IPAddress.Parse("81.12.31.192"), IPAddress.Parse("81.12.31.254")),
			new IPAddressRange(IPAddress.Parse("91.107.165.81"), IPAddress.Parse("91.107.165.81")),
		};
			_logger = logger;
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			var remoteIp = context.HttpContext.Connection.RemoteIpAddress;


			//if (!_allowedRanges.Any(r => r.Contains(remoteIp)))
			//{
			//    context.Result = new ContentResult
			//    {
			//        StatusCode = 403,
			//        Content = "Forbidden: IP not allowed."
			//    };

			//    await _logger.ErrorAsync($"Forbidden: IP not allowed for torob api: {remoteIp?.ToString()}");

			//}
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
		}
	}

}
