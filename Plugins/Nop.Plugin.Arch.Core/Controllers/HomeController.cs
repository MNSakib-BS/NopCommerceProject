using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;

namespace Nop.Plugin.Arch.Core.Controllers;

public partial class HomeController : BasePublicController
{
    public virtual IActionResult Index()
    {
        return View();
    }
}