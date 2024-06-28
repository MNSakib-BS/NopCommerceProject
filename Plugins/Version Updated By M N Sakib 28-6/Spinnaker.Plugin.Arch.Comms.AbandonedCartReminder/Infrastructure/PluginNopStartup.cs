using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Infrastructure;
public class PluginNopStartup : INopStartup
{
    public int Order => 11;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
 //       services.AddScoped<ICustomAttributeService, CustomService>();
 //       services.AddScoped<ICustomModelFactory, CustomModelFactory>();
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ViewLocationExpander());
        });
    }
}
