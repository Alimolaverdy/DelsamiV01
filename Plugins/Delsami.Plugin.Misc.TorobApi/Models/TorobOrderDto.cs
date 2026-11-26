using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.TorobApi.Models
{
	public class TorobOrderDto
	{
		public DateTime purchase_timestamp { get; set; }

		public DateTime last_updated_timestamp { get; set; }

		public string torob_clid { get; set; }

		public string status { get; set; }

		public string? psp { get; set; }

		public int? order_value { get; set; }

		public int? shipping_amount { get; set; }

		public string? phone_number { get; set; }

		public List<TorobOrderProductDto> products { get; set; } 
	}
}
