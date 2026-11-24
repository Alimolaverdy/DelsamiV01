using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Data.Mapping.Mapping;

public class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string> {
    {
        typeof(HtmlWidget),
        "SS_HW_HtmlWidget"
    } };

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
}
