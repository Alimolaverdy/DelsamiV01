using Delsami.Plugin.Payments.Digipay.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Payments.Digipay.Data.Mapping;

public class NameCompatibility : INameCompatibility
{
	public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
	{
		{ typeof(DigiPayRefundRecord), $"D_PDR_{nameof(DigiPayRefundRecord)}"},
	};

	public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
}
