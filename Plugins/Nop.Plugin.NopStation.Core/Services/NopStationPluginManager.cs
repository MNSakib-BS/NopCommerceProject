using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public class NopStationPluginManager : PluginManager<INopStationPlugin>, INopStationPluginManager
    {
        #region Fields

        private readonly NopStationCoreSettings _nopStationCoreSettings;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public NopStationPluginManager(NopStationCoreSettings nopStationCoreSettings,
            IPluginService pluginService,
            ILocalizationService localizationService,
            ICustomerService customerService) : base(customerService, pluginService)
        {
            _nopStationCoreSettings = nopStationCoreSettings;
            _localizationService = localizationService;
        }

        #endregion

        public virtual async Task<IList<INopStationPlugin>> LoadNopStationPluginsAsync(Customer customer = null, string pluginSystemName = "", 
            int storeId = 0, string widgetZone = null)
        {
            var systemNames = !string.IsNullOrWhiteSpace(pluginSystemName) ? new List<string> { pluginSystemName } :
                _nopStationCoreSettings.ActiveNopStationSystemNames;

            var nopStationPlugins = LoadActivePluginsAsync(systemNames, customer, storeId);

            return await nopStationPlugins;
        }

        public virtual async Task<IPagedList<KeyValuePair<string, string>>> LoadPluginStringResourcesAsync(string pluginSystemName = "", 
            string keyword = "", int languageId = 0, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var nopStationPlugins = await LoadNopStationPluginsAsync(pluginSystemName: pluginSystemName, storeId: storeId);
            var originnalResources = new List<KeyValuePair<string, string>>();
            foreach (var plugin in nopStationPlugins)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    originnalResources.AddRange(plugin.PluginResouces().OrderBy(x => x.Key).ToList());
                else
                    originnalResources.AddRange(plugin.PluginResouces()
                        .Where(x => x.Key.ToLower().Contains(keyword.ToLower())).OrderBy(x => x.Key).ToList());
            }

            var total = originnalResources.Count();
            originnalResources = originnalResources.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var pagedResources = new List<KeyValuePair<string, string>>();
            foreach (var item in originnalResources)
            {
                var resource = await _localizationService.GetResourceAsync(item.Key, languageId, false, "", true);
                if (string.IsNullOrWhiteSpace(resource))
                {
                    await _localizationService.AddOrUpdateLocaleResourceAsync(item.Key, item.Value);
                    resource = item.Value;
                }
                pagedResources.Add(new KeyValuePair<string, string>(item.Key, resource));
            }

            return new PagedList<KeyValuePair<string, string>>(pagedResources, pageIndex, pageSize, total);
        }
    }
}
