using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
                viewLocations = new[] { $"/Plugins/NopStation.Core/Views/{{1}}/{{0}}.cshtml" }.Concat(viewLocations);

            return viewLocations;
        }
    }
}
