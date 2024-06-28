using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority => 100;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapControllerRoute("Spinnaker.Plugin.HealthMonitor.Admin.Configure",
            pattern: "Admin/HealthMonitor/Configure",
            defaults: new { controller = "Health", action = "Configure", area = AreaNames.ADMIN });

            routeBuilder.MapControllerRoute("Spinnaker.Plugin.HealthMonitor.Admin.SendTest",
            pattern: "Admin/HealthMonitor/SendTest",
            defaults: new { controller = "Health", action = "SendTest", area = AreaNames.ADMIN });
        }
    }
}
