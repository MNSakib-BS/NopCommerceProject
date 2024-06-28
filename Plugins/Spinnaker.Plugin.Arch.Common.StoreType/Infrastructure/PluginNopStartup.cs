using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            /*services.AddScoped<ICustomAttributeService, CustomAttributeService>();
            services.AddScoped<ICustomModelFactory, CustomModelFactory>();*/
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