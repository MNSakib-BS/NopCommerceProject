using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;
using Nop.Services.Security;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public class HangfireDashAuthFilter : IDashboardAuthorizationFilter
{

    public bool Authorize([NotNull] DashboardContext context)
    {
        var permissionService = EngineContext.Current.Resolve<IPermissionService>();
        return permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks);
    }
}
