using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Spinnaker.Plugin.Arch.Shipping.AddressPicker.Infrastructure;
public class ViewLocationExpander : IViewLocationExpander
{
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] { $"/Plugins/Spinnaker.Plugin.Arch.AddressPicker/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] { $"/Plugins/Spinnaker.Plugin.Arch.AddressPicker/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        return viewLocations;
    }

    public void PopulateValues(ViewLocationExpanderContext context){ }
}
