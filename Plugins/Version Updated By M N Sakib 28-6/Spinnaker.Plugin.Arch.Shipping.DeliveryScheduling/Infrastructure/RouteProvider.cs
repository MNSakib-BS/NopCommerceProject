using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            // Admin
            endpointRouteBuilder.MapControllerRoute("Plugin.Widget.DeliveryScheduling.Configure",
                               "Admin/Controllers/DeliveryScheduling/Configure",
                                new { controller = "DeliveryScheduling", action = "Configure", areas = "Admin" });

            // Other routes can be added here if needed
        }

        public int Priority => 1;
    }
}


