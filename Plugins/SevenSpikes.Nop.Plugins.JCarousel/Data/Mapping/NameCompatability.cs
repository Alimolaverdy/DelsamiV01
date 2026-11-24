using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;

namespace SevenSpikes.Nop.Plugins.JCarousel.Data.Mapping;

public class NameCompatability : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
    {
        { typeof(Carousel), "SS_JC_JCarousel" }
    };

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
}
