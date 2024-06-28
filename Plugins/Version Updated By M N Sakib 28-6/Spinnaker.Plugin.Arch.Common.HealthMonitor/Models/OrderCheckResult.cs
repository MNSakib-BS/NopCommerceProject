using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Models;
public class OrderCheckResult
{
    public bool HasUnacceptedOrders { get; internal set; }
}
