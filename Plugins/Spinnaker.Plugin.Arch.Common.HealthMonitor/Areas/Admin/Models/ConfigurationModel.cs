using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Areas.Admin.Models;
public record ConfigurationModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }
    [NopResourceDisplayName("Admin.Spinnaker.HealthMonitor.MonitoringHostURL")]
    public string MonitoringHostURL { get; set; }
    [NopResourceDisplayName("Admin.Spinnaker.HealthMonitor.SiteKey")]
    public string SiteKey { get; set; }
}
