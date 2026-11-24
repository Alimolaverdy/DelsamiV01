using System.Collections.Generic;
using Newtonsoft.Json;

namespace SevenSpikes.Nop.Plugins.JCarousel.Domain;

public class ResponsiveBreakpointDTO
{
    [JsonProperty("breakpoint")]
    public int Breakpoint { get; set; }

    [JsonProperty("settings")]
    public IDictionary<string, object> Settings { get; set; }
}
