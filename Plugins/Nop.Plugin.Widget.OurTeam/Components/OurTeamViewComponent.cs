using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.OurTeam.Components;
public class OurTeamViewComponent : NopViewComponent
{
    //lagbena
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        return View("~/Plugins/Widgets.OurTeam/Views/OurTeam/Default.cshtml");

    }
}
