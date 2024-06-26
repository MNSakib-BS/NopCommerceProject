using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nop.Core;
using System.Threading.Tasks;
using System.Threading;
using System;
using Nop.Services.Payments;
using Nop.Core.Domain.Customers;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure.Checks
{
    public class PaymentProviderHealthCheck
    {
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        public PaymentProviderHealthCheck(IPaymentPluginManager paymentPluginManager, IWorkContext workContext, IStoreContext storeContext) 
        {
            _paymentPluginManager = paymentPluginManager;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var paymentMethod =await _paymentPluginManager.LoadPluginBySystemNameAsync("PAYGATE", await _workContext.GetCurrentCustomerAsync(), _storeContext.GetCurrentStoreAsync().Id) ?? throw new NopException("Payment method couldn't be loaded");

            return await Task.Run(() =>
            {
                try
                {
                    if (paymentMethod.SupportsHealthCheck)
                    {
                        var healthCheckResult = paymentMethod.GetHealthCheckResult();

                        if (healthCheckResult.IsHealthy)
                        {
                            return HealthCheckResult.Healthy($"Payment provider is successfully connected!  {healthCheckResult.ResponseMessage}");
                        }
                        else
                        {
                            return HealthCheckResult.Unhealthy($"Payment provider failed to connect!  {healthCheckResult.ResponseMessage}");
                        }
                       
                    }

                    return HealthCheckResult.Unhealthy("Payment provider is not supported for health check!");
                }

                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"Payment provider is unhealthy {ex}");
                }
            });
        }
    }
}
