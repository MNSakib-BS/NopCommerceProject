using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.MegaMenu.Factories;
using Nop.Plugin.NopStation.MegaMenu.Services;

namespace Nop.Plugin.NopStation.MegaMenu.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMegaMenuModelFactory, MegaMenuModelFactory>();
            services.AddScoped<IMegaMenuCoreService, MegaMenuCoreService>();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 1;
    }
}