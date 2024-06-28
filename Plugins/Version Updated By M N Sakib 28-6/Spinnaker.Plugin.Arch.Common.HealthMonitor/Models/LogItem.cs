using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.CustomProperties;


namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Models;
public class LogItem
{
    public int LogItemID { get; set; }
    public virtual int ApplicationInstanceID { get; set; }
    public virtual ApplicationInstance ApplicationInstance { get; set; }
    public string Level { get; set; }
    public string LogMessage { get; set; }
    public DateTime EventDate { get; set; }
    public string ExceptionDetail { get; set; }
}
public class LogEvent
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string RenderedMessage { get; set; }
    public string SiteKey { get; set; }
    public Properties[] Properties { get; set; }
    public string Exception { get; set; }
}
