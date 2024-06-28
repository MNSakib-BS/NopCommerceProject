using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public class NopScheduledTaskJob
{
    private readonly IScheduleTaskService _scheduleTaskService;

    public NopScheduledTaskJob(IScheduleTaskService scheduleTaskService)
    {
        _scheduleTaskService = scheduleTaskService;
    }


    public void ExecuteTask(string taskType)
    {
        var scheduleTask = _scheduleTaskService.GetTaskByType(taskType);
        if (scheduleTask == null)
            //schedule task cannot be loaded
            return;
        var task = new Task(scheduleTask);
        task.Execute();
    }
}
