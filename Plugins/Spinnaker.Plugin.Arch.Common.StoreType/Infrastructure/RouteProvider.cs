using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 100;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Spinnaker.Plugin.Arch.StoreType.StoreTypeRoute",
            pattern: "Admin/StoreType",
            defaults: new { controller = "StoreType", action = "Index", area = AreaNames.ADMIN });
        }
    }
}
