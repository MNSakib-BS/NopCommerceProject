using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.Theme.Arch;
public class ArchPermissionProvider : IPermissionProvider
{
    public static readonly PermissionRecord ManageArch = new PermissionRecord { Name = "Arch theme. Manage Arch theme", SystemName = "ManageNopStationArch", Category = "NopStation" };

    public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
    {
        return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        ManageArch
                    }
                )
            };
    }

    public IEnumerable<PermissionRecord> GetPermissions()
    {
        return new[]
            {
                ManageArch
            };
    }
}
