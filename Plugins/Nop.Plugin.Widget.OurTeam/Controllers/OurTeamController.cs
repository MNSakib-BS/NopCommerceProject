using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.OurTeam.Factories;
using Nop.Plugin.Widgets.OurTeam.Models;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.OurTeam.Controllers
{
    public class OurTeamController : BasePluginController
    {
        private readonly IEmployeesModelFactory _employeesModelFactory;

        public OurTeamController(IEmployeesModelFactory employeesModelFactory)
        {
            _employeesModelFactory = employeesModelFactory;
        }

        public async Task<IActionResult> IndexAsync()
        {
           var model = await _employeesModelFactory.PrepareAllEmployeesAsync();

        return View(model);
        }
    }
}
