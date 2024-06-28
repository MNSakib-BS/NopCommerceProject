using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public class PreventConcurrentExecutionJobAttribute : JobFilterAttribute, IClientFilter, IServerFilter
{
    public void OnCreating(CreatingContext filterContext)
    {
        var jobs = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, 100);

        if (jobs == null || !jobs.Any())
            return;
        if (!filterContext.Job.Args.Any())
            return;
        if (jobs.Any(x => x.Value.Job.Type == filterContext.Job.Type && string.Join(".", x.Value.Job.Args) == string.Join(".", filterContext.Job.Args)))
        {
            filterContext.Canceled = true;
        }
    }

    public void OnPerformed(PerformedContext filterContext) { }

    void IClientFilter.OnCreated(CreatedContext filterContext) { }

    void IServerFilter.OnPerforming(PerformingContext filterContext) { }
}
