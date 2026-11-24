using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace SevenSpikes.Nop.Plugins.JCarousel.Infrastructure;

public class NopStartup : INopStartup
{
    public int Order => 2001;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }
}
