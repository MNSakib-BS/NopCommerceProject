using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Areas.Admin.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => 1;

    public void Configure(IApplicationBuilder application)
    {
        throw new NotImplementedException();
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
       // services.AddScoped<ICustomModelFactory, CustomModelFactory>();
    }
}
