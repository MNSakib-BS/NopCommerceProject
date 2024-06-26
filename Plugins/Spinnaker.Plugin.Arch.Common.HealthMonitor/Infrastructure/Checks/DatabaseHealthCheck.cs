using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nop.Core.Domain.Stores;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure.Checks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IRepository<Store> _storeRepository;

        public DatabaseHealthCheck(IRepository<Store> storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = _storeRepository.Table.Any();
                    return HealthCheckResult.Healthy($"Database accessible: {result}");
                }
                catch (Exception ex)
                {
                    return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
                }
            });
        }
    }
}
