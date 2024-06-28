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
using Nop.Plugin.NopStation.MegaMenu.Factories;
using Nop.Plugin.NopStation.MegaMenu.Services;

namespace Nop.Plugin.NopStation.MegaMenu.Areas.Admin.Infrastructure;
public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryIconService, CategoryIconService>();
        services.AddScoped<ICategoryIconModelFactory, CategoryIconModelFactory>();
       
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 1;
}