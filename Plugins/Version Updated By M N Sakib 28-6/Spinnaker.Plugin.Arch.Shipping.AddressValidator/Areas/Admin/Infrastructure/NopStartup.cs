using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator.Areas.Admin.Infrastructure
{
    public class NopStartup :INopStartup
    {
        

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // services.AddScoped<ICustomModelFactory, CustomModelFactory>() > ();
            // services.AddScoped<ICustomAttributeService, CustomAttributeService>() > ();
        }

        public void Configure(IApplicationBuilder application){ }

        public int Order => 1;
    }
}
