using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Nop.Plugin.Widgets.OurTeam.Infrastructure;
internal class RouteProviderIRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute("OurTeam", "OurTeam",
            new { controller = "OurTeam", action = "Index", area = "" });
    }

    public int Priority => 0;
}