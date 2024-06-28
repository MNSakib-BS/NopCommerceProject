using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using Nop.Services.Configuration;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure.Checks
{
    public class ApplicationHealthCheck : IHealthCheck
    {
        private readonly ISettingService Configuration;

        public ApplicationHealthCheck(ISettingService configuration)
        {
            Configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                var settings = Configuration.LoadSetting<HealthMonitorSettings>();
                var siteKey = settings.SiteKey;

                var keyDictionary = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(siteKey))
                {
                    keyDictionary.Add(Constants.ConstSiteKey, siteKey);
                }
                var keyData = new ReadOnlyDictionary<string, object>(keyDictionary);
                return HealthCheckResult.Healthy($"<a class='btn btn-primary' href='/exception/view?sitekey={siteKey}'>View Logs<a/>", keyData);
            });
        }
    }
}
