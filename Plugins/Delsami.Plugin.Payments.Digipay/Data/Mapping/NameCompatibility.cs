using ZarIran.Plugin.Payments.Digipay.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.Digipay.Data.Mapping;

public class NameCompatibility : INameCompatibility
{
	public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
	{
		{ typeof(DigiPayRefundRecord), $"ZI_PDR_{nameof(DigiPayRefundRecord)}"},
	};

	public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
}
