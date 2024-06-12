using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.OurTeam.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context){}

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var pluginViewLocations = new[]
            {
                "~/Plugins/Widgets.OurTeam/Views/{1}/{0}.cshtml",
                "~/Plugins/Widgets.OurTeam/Views/Shared/{0}.cshtml"
            };

            
            var locations = pluginViewLocations.Concat(viewLocations);

           
            if (context.Values.TryGetValue("nop.themename", out var themeName))
            {
       
                var themeViewLocations = new[]
                {
                    $"/Plugins/Widgets.OurTeam/Themes/{themeName}/Views/Shared/{{0}}.cshtml",
                    $"/Plugins/Widgets.OurTeam/Themes/{themeName}/Views/{{1}}/{{0}}.cshtml"
                };
                locations = themeViewLocations.Concat(locations);
            }

            return locations;
        }
    }
}
