using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;

namespace Nop.Plugin.NopStation.Core.Helpers
{
    public static class NopStationHelpers
    {
        public static async Task NopStationPluginInstallAsync<T>(this T plugin, IPermissionProvider provider) where T : INopStationPlugin
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var keyValuePairs = plugin.PluginResouces();
            foreach (var keyValuePair in keyValuePairs)
            {
                await localizationService.AddOrUpdateLocaleResourceAsync(keyValuePair.Key, keyValuePair.Value);
            }

            if (provider != null)
            {
                var permissionService = EngineContext.Current.Resolve<IPermissionService>();
                await permissionService.InstallPermissionsAsync(provider);
            }

            var nopStationCoreSettings = EngineContext.Current.Resolve<NopStationCoreSettings>();
            if (!nopStationCoreSettings.ActiveNopStationSystemNames?.Any(x => x == plugin.PluginDescriptor.SystemName) ?? false)
            {
                nopStationCoreSettings.ActiveNopStationSystemNames.Add(plugin.PluginDescriptor.SystemName);
                await settingService.SaveSettingAsync(nopStationCoreSettings);
            }

            EnablePlugin(plugin, settingService);
        }

        public static async Task NopStationPluginUninstallAsync<T>(this T plugin, IPermissionProvider provider) where T : INopStationPlugin
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var keyValuePairs = plugin.PluginResouces();
            foreach (var keyValuePair in keyValuePairs)
            {
                await localizationService.DeleteLocaleResourceAsync(keyValuePair.Key);
            }

            if (provider != null)
            {
                var permissionService = EngineContext.Current.Resolve<IPermissionService>();
                await permissionService.UninstallPermissionsAsync(provider);
            }

            var nopStationCoreSettings = EngineContext.Current.Resolve<NopStationCoreSettings>();
            if (nopStationCoreSettings.ActiveNopStationSystemNames.Any(x => x == plugin.PluginDescriptor.SystemName))
            {
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                nopStationCoreSettings.ActiveNopStationSystemNames.Remove(plugin.PluginDescriptor.SystemName);
                await settingService.SaveSettingAsync(nopStationCoreSettings);
            }
        }

        public static async Task EnablePlugin<T>(T plugin, ISettingService settingService) where T : INopStationPlugin
        {
            try
            {
                var pluginIsActive = false;
                switch (plugin)
                {
                    case IPaymentMethod paymentMethod:
                        var paymentPluginManager = EngineContext.Current.Resolve<IPaymentPluginManager>();
                        pluginIsActive = paymentPluginManager.IsPluginActive(paymentMethod);
                        if (!pluginIsActive)
                        {
                            var paymentSettings = EngineContext.Current.Resolve<PaymentSettings>();
                            paymentSettings.ActivePaymentMethodSystemNames.Add(plugin.PluginDescriptor.SystemName);
                            await settingService.SaveSettingAsync(paymentSettings);
                        }

                        break;
                    case IShippingRateComputationMethod shippingRateComputationMethod:
                        var shippingPluginManager = EngineContext.Current.Resolve<IShippingPluginManager>();
                        pluginIsActive = shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                        if (!pluginIsActive)
                        {
                            var shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
                            shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(plugin.PluginDescriptor.SystemName);
                            await settingService.SaveSettingAsync(shippingSettings);
                        }

                        break;
                    case IPickupPointProvider pickupPointProvider:
                        var pickupPluginManager = EngineContext.Current.Resolve<IPickupPluginManager>();
                        pluginIsActive = pickupPluginManager.IsPluginActive(pickupPointProvider);
                        if (!pluginIsActive)
                        {
                            var shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
                            shippingSettings.ActivePickupPointProviderSystemNames.Add(plugin.PluginDescriptor.SystemName);
                            await settingService.SaveSettingAsync(shippingSettings);
                        }

                        break;
                    case IExternalAuthenticationMethod externalAuthenticationMethod:
                        var authenticationPluginManager = EngineContext.Current.Resolve<IAuthenticationPluginManager>();
                        pluginIsActive = authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                        if (!pluginIsActive)
                        {
                            var externalAuthenticationSettings = EngineContext.Current.Resolve<ExternalAuthenticationSettings>();
                            externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(plugin.PluginDescriptor.SystemName);
                            await settingService.SaveSettingAsync(externalAuthenticationSettings);
                        }

                        break;
                    case IWidgetPlugin widgetPlugin:
                        var widgetPluginManager = EngineContext.Current.Resolve<IWidgetPluginManager>();
                        pluginIsActive = widgetPluginManager.IsPluginActive(widgetPlugin);
                        if (!pluginIsActive)
                        {
                            var widgetSettings = EngineContext.Current.Resolve<WidgetSettings>();
                            widgetSettings.ActiveWidgetSystemNames.Add(plugin.PluginDescriptor.SystemName);
                            await settingService.SaveSettingAsync(widgetSettings);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>()
                    .Error($"Failed to enable {plugin.PluginDescriptor.SystemName}: {ex.Message}", ex);
            }
        }
    }
}
