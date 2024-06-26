using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Infrastructure;
public class RouteProvider : IRouteProvider
{
    public int Priority => 100;

    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute("Spinnaker.Plugin.Arch.AbandonedCartReminder.AbandonedCartReminderRoute",
        pattern: "Admin/AbandonedCartReminder",
        defaults: new { controller = "AbandonedCartReminder", action = "Index", area = AreaNames.ADMIN });

    }
}
