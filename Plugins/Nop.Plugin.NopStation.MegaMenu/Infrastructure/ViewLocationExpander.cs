using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.MegaMenu.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";
        //private const string MEGAMENU_KEY = "nopstation.megamenu";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //if (context.ViewName == "Components/TopMenu/Default")
            //{
            //    var megaMenuSettings = EngineContext.Current.Resolve<MegaMenuSettings>();
            //    if (megaMenuSettings.EnableMegaMenu)
            //        context.Values[MEGAMENU_KEY] = "true";
            //}
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new string[] {
                    $"/Plugins/NopStation.MegaMenu/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.MegaMenu/Areas/Admin/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }
            else
            {
                //if (context.Values.TryGetValue(MEGAMENU_KEY, out _))
                //    viewLocations = new string[] { "~/Plugins/NopStation.MegaMenu/Views/Shared/TopMenu.cshtml" };
                //else if (context.Values.TryGetValue(THEME_KEY, out string theme))
                //{
                //    viewLocations = new string[] {
                //        $"/Plugins/NopStation.MegaMenu/Views/{{1}}/{{0}}.cshtml",
                //        $"/Plugins/NopStation.MegaMenu/Views/Shared/{{0}}.cshtml"
                //    }.Concat(viewLocations);

                //    viewLocations = new string[] {
                //        $"/Plugins/NopStation.MegaMenu/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                //        $"/Plugins/NopStation.MegaMenu/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                //    }.Concat(viewLocations);
                //}

                viewLocations = new string[] {
                        $"/Plugins/NopStation.MegaMenu/Views/{{1}}/{{0}}.cshtml",
                        $"/Plugins/NopStation.MegaMenu/Views/Shared/{{0}}.cshtml"
                    }.Concat(viewLocations);

                if (context.Values.TryGetValue(THEME_KEY, out string theme))
                    viewLocations = new string[] {
                            $"/Plugins/NopStation.MegaMenu/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                            $"/Plugins/NopStation.MegaMenu/Themes/{theme}/Views/Shared/{{0}}.cshtml"
                        }.Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
