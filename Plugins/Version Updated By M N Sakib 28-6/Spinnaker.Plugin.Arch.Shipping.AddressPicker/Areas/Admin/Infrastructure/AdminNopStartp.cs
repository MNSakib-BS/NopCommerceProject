using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Spinnaker.Plugin.Arch.Shipping.AddressPicker.Areas.Admin.Infrastructure;
public class AdminNopStartp : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutofac(containerBuilder =>
        {
            /*containerBuilder.RegisterType<CustomService>().As<ICustomAttributeService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<CustomModelFactory>().As<ICustomModelFactory>().InstancePerLifetimeScope();*/
        });
    }

    public void Configure(IApplicationBuilder application){ }

    public int Order => 1;
}
