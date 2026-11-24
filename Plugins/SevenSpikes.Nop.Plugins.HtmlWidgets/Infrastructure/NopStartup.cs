using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure;

public class NopStartup : INopStartup
{
    public int Order => 2001;

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void Configure(IApplicationBuilder application)
    {
    }
}
