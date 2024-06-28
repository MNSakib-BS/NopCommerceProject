using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.NopStation.Core.Services;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<NopStationContext>().As<INopStationContext>().InstancePerLifetimeScope();
            builder.RegisterType<NopStationPluginManager>().As<INopStationPluginManager>().InstancePerLifetimeScope();
            builder.RegisterType<NopStationCoreService>().As<INopStationCoreService>().InstancePerLifetimeScope();
            builder.RegisterType<NopStationLicenseService>().As<INopStationLicenseService>().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}
