using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.ScheduleTasks;

namespace Nop.Plugin.Arch.Core.Services.Tasks;
public interface IArchScheduleTaskService
{
    /// <summary>
    /// Updates or inserts the task
    /// </summary>
    /// <param name="task">Task</param>
    Task InsertUpdateTaskAsync(ScheduleTask task);
}
