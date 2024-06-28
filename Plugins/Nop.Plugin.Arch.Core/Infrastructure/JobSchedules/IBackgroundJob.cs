using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public interface IBackgroundJob
{
    void Execute();
}
