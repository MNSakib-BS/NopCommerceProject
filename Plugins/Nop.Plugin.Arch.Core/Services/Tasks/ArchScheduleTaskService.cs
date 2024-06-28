using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Arch.Core.Services.Tasks;
public class ArchScheduleTaskService : IArchScheduleTaskService
{
    #region Fields

    private readonly IScheduleTaskService _scheduleTaskService;

    #endregion

    #region Ctor

    public ArchScheduleTaskService(IScheduleTaskService scheduleTaskService)
    {
        _scheduleTaskService = scheduleTaskService;
    }

    #endregion

    #region Methods

    public virtual async Task InsertUpdateTaskAsync(ScheduleTask task)
    {
        if (task != null && !string.IsNullOrEmpty(task.Type))
        {
            if (await _scheduleTaskService.GetTaskByTypeAsync(task.Type) != null)
            {
                await _scheduleTaskService.UpdateTaskAsync(task);
            }
            else
            {
               await _scheduleTaskService.InsertTaskAsync(task);
            }
        }
    }

    #endregion


}
