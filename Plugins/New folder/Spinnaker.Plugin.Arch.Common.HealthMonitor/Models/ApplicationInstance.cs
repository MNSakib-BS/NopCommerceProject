using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Models;
public class ApplicationInstance
{
    public int ApplicationInstanceID { get; set; }
    public string SiteKey { get; set; }
    public virtual List<LogItem> Logs { get; set; }
    public int ConfigurationID { get; set; }

    public ApplicationInstance()
    {
        Logs = new List<LogItem>();
    }
}
