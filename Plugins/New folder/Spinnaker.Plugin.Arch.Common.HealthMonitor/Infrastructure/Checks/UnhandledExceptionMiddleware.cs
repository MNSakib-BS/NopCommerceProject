using Microsoft.AspNetCore.Http;
using Nop.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Nop.Services.Events;
using System.Net.Http;
using Nop.Services.Customers;
using Spinnaker.Plugin.Arch.Common.HealthMonitor.Models;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Logging;
using Nop.Core.Events;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure.Checks
{
    public interface IUnhandledRequestHandler
    {
        Task Invoke(HttpContext httpContext);
    }

    public class UnhandledExceptionMiddleware : IUnhandledRequestHandler
    {
        protected readonly IUserAgentHelper _userAgentHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly ICustomerService _customerService;
        public UnhandledExceptionMiddleware(IUserAgentHelper userAgentHelper, IHttpContextAccessor httpContextAccessor, ILogger logger, RequestDelegate next, ICustomerService customerService)
        {
            _userAgentHelper = userAgentHelper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _next = next;
            _customerService = customerService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unhandled Exception", ex);
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
                eventPublisher.Publish(new ErrorHandledEvent($"Unhandled Exception Occurred", ex));
            }
        }
    }
}
