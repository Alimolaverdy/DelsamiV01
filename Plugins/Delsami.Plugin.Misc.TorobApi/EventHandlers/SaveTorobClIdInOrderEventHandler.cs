using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Delsami.Plugin.Misc.TorobApi.EventHandlers
{
	public class SaveTorobClIdInOrderEventHandler : IConsumer<OrderPlacedEvent>
	{
		private readonly TorobApiSettings _torobApiSettings;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IOrderService _orderService;

		public SaveTorobClIdInOrderEventHandler(TorobApiSettings torobApiSettings, IHttpContextAccessor httpContextAccessor, IOrderService orderService)
		{
			_torobApiSettings = torobApiSettings;
			_httpContextAccessor = httpContextAccessor;
			_orderService = orderService;
		}

		public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
		{
			if (_torobApiSettings.IsActive)
			{
				if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("torob_clid", out var torobClId))
				{
					//using cardName for torob-Clid
					eventMessage.Order.CardName = torobClId;

					await _orderService.UpdateOrderAsync(eventMessage.Order);
				}

			}
		}
	}
}
