using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Infrastructure;
public class ViewLocationExpander : IViewLocationExpander
{
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] { $"/Plugins/Arch.AbandonedCartReminder/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] { $"/Plugins/Arch.AbandonedCartReminder/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }

        return viewLocations;
    }

    public void PopulateValues(ViewLocationExpanderContext context){ }
}
