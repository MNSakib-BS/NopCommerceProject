using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public interface IJob
{
    void Execute(int storeId, bool multiStore = false);

    /// <summary>
    /// Stops a tasks from running.
    /// </summary>
    void Stop();

    /// <summary>
    /// Store Id the task is running on.
    /// </summary>
    int RunningOnStoreId { get; }
}
