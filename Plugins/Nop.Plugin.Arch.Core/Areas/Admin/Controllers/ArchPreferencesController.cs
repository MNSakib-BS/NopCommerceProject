using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Controllers;

namespace Nop.Plugin.Arch.Core.Areas.Admin.Controllers;

public partial class ArchPreferencesController : BaseAdminController
{
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ArchPreferencesController(IGenericAttributeService genericAttributeService,
        IWorkContext workContext)
    {
        _genericAttributeService = genericAttributeService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    [HttpPost]
    public virtual async Task<IActionResult> SavePreference(string name, bool value)
    {
        //permission validation is not required here
        ArgumentException.ThrowIfNullOrEmpty(name);

        await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), name, value);

        return Json(new
        {
            Result = true
        });
    }

    #endregion
}