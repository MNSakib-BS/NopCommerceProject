using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.Core
{
    public class CorePermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ManageLicense = new PermissionRecord { Name = "NopStation core. Manage license", SystemName = "ManageNopStationCoreLicense", Category = "NopStation" };
        public static readonly PermissionRecord ManageConfiguration = new PermissionRecord { Name = "NopStation core. Manage configuration", SystemName = "ManageNopStationCoreConfiguration", Category = "NopStation" };

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageLicense,
                ManageConfiguration
            };
        }

        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        ManageLicense,
                        ManageConfiguration,
                    }
                )
            };
        }

    }
}
