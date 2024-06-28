using Microsoft.Extensions.Diagnostics.HealthChecks;
using Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Misc.HealthMonitor.Infrastructure.Checks
{
    public class SalesOrderCheck : IHealthCheck
    {
        private readonly IHealthCheckService _healthCheckService;
        
        public SalesOrderCheck(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var salesOrderResult = await _healthCheckService.CheckOrderHealth();

            if (salesOrderResult.HasUnacceptedOrders)
            {
                return HealthCheckResult.Unhealthy("Sales Orders failed to process, check eStore Sales Orders");
            }

            return HealthCheckResult.Healthy("Sales Orders healthy");
        }
    }
}
