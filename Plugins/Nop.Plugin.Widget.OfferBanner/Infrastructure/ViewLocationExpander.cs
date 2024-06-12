using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widget.OfferBanner.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[]
                {
                $"/Plugins/Widgets.OfferBanner/Areas/Admin/Views/Shared/{{0}}.cshtml",
                $"/Plugins/Widgets.OfferBanner/Areas/Admin/Views/{{1}}/{{0}}.cshtml"
            }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[]
                {
                $"/Plugins/Widgets.OfferBanner/Views/Shared/{{0}}.cshtml",
                $"/Plugins/Widgets.OfferBanner/Views/{{1}}/{{0}}.cshtml"
            }.Concat(viewLocations);

                if (context.Values.TryGetValue("nop.themename", out var themeName))
                {
                    viewLocations = new[]
                {
                $"/Plugins/Widgets.OfferBanner/Themes/{themeName}/Views/Shared/{{0}}.cshtml",
                $"/Plugins/Widgets.OfferBanner/Themes/{themeName}/Views/{{1}}/{{0}}.cshtml"
            }.Concat(viewLocations);
                }
            }
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context) { }
    }
}
