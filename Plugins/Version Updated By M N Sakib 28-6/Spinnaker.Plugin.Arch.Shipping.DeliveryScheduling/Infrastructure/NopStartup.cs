using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Factories;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Factories;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Infrastructure
{
    public class NopStartup : INopStartup
    {
        // Add services to the application
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register services using AddScoped
            services.AddScoped<ITimeDeliveryService, TimeDeliveryService>();
            services.AddScoped<IShippingMethodService, ShippingMethodService>();

            services.AddScoped<IAvailableDeliveryDateTimeRangeService, AvailableDeliveryDateTimeRangeService>();
            services.AddScoped<IOrderShippingMethodCapacityMappingService, OrderShippingMethodCapacityMappingService>();
            services.AddScoped<IShippingMethodCapacityService, ShippingMethodCapacityService>();
            services.AddScoped<IAvailableDeliveryTimeRangeService, AvailableDeliveryTimeRangeService>();
            services.AddScoped<IOrderShippingMethodCapacityMappingService, OrderShippingMethodCapacityMappingService>();

            services.AddScoped<IDeliveryScheduleConfigurationFactory, DeliveryScheduleConfigurationFactory>();
            services.AddScoped<IDeliveryScheduleFactory, DeliveryScheduleFactory>();
        }

     
        public void Configure(IApplicationBuilder application)
        {
          
        }

        public int Order
        {
            get { return 500; }
        }
    }
}
