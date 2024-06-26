using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Spinnaker.Plugin.Arch.Shipping.AddressPicker.Infrastructure;

namespace Spinnaker.Plugin.Arch.AddressPicker.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
            services.AddAutofac(containerBuilder =>
            {
                /*containerBuilder.RegisterType<CustomService>().As<ICustomAttributeService>().InstancePerLifetimeScope();
                containerBuilder.RegisterType<CustomModelFactory>().As<ICustomModelFactory>().InstancePerLifetimeScope();*/
            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}