using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure
{
    public class Constants
    {
        public const string ConstSiteKey = "site-key";
        public const string ApplicationNameKey = "app-name";
        public const string ConstDegradedThreshold = "DegradedThreshold";
        public const string ConstUnhealtyThreshold = "UnhealtyThreshold";
        public const string HealthCheckRoute = "/health";
    }
}
