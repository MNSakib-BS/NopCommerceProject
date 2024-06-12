using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widget.OfferBanner.Infrastructure
{
    internal class RouterProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Widget.OfferBanner.Configure",
                $"/Admin/OfferBanner/Configure",
                new { area = "Admin", controller = "OfferBanner", action = "Configure" });

        }

        
    }
}
