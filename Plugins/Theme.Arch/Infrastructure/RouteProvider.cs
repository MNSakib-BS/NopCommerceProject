using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.NopStation.Theme.Arch.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 100;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("QuantityUpdate", $"quantityupdate",
                new { controller = "Arch", action = "ItemQuantityUpdate" });
        }
    }
}
