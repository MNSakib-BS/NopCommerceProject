using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Spinnaker.Plugin.Arch.AbandonedCartReminder.Services;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Areas.Admin.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order =>1;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAbandonedCartReminderService, AbandonedCartReminderService>();
        services.AddScoped<IPluginDefaults, PluginDefaults>();
    }
}
