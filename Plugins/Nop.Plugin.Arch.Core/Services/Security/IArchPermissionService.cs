using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Security;

namespace Nop.Plugin.Arch.Core.Services.Security;
public interface IArchPermissionService
{
    /// <summary>
    /// Updates the permission
    /// </summary>
    /// <param name="permission">Permission</param>
    Task InsertUpdatePermissionRecordAsync(PermissionRecord permission);
}
