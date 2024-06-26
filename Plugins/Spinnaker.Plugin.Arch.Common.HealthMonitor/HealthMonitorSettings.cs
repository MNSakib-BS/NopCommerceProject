using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor;
public class HealthMonitorSettings : ISettings
{
    public string MonitoringHostURL { get; set; }
    public string SiteKey { get; set; }
}
