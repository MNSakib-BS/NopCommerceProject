using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Arch.Core.Services.Security;
public class ArchPermissionService : IArchPermissionService
{
    #region Fields

    private readonly IPermissionService _permissionService;


    #endregion

    #region Ctor

    public ArchPermissionService(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    #endregion

    #region Methods
    public virtual async Task InsertUpdatePermissionRecordAsync(PermissionRecord permission)
    {
        if (permission != null && !string.IsNullOrEmpty(permission.SystemName))
        {
            if (await _permissionService.GetPermissionRecordByIdAsync(permission.Id) != null)
            {
              await _permissionService.UpdatePermissionRecordAsync(permission);
            }
            else
            {
                await _permissionService.InsertPermissionRecordAsync(permission);
            }
        }
    }

    #endregion
}
