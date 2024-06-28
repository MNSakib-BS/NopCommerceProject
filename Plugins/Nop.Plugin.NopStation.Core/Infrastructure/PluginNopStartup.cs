using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Services;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
            services.AddScoped<INopStationContext, NopStationContext>();
            services.AddScoped<INopStationPluginManager, NopStationPluginManager>();
            services.AddScoped<INopStationCoreService, NopStationCoreService>();
            services.AddScoped<INopStationLicenseService, NopStationLicenseService>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}