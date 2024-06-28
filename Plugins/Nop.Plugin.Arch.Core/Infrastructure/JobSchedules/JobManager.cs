using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public static class JobManager
{
    public static void RunNow(System.Linq.Expressions.Expression<Action> action)
    {
        BackgroundJob.Enqueue(action);
    }

    public static void RunAt(System.Linq.Expressions.Expression<Action> action, TimeSpan timer)
    {
        BackgroundJob.Schedule(action, timer);
    }

    public static void Log(string message)
    {

    }
}
