using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Arch.Core.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                            $"~/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                            $"~/Areas/Admin/Views/Shared/{{0}}.cshtml"
                        }.Concat(viewLocations);
            }
            else
            {
                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                {
                    if (context.ControllerName.StartsWith("Arch"))
                    {
                        string controllerName = context.ControllerName.Substring(4);

                        viewLocations = new[] {
                        $"~/Themes/{theme}/Views/{controllerName}/{context.ViewName}.cshtml",
                        $"~/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                        $"~/Views/{controllerName}/{context.ViewName}.cshtml",
                        $"~/Views/Shared/{{0}}.cshtml"
                        }
                        .Concat(viewLocations);

                    }
                    else
                    {
                        viewLocations = new[] {
                        $"~/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"~/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                        }
                        .Concat(viewLocations);
                    }
                }
                else
                {
                    viewLocations = new[] {
                            $"~/Views/{{1}}/{{0}}.cshtml",
                            $"~/Views/Shared/{{0}}.cshtml"
                        }.Concat(viewLocations);
                }
            }
            return viewLocations;
        }
    }
}
