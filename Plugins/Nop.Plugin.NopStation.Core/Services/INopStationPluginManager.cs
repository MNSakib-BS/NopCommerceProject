using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.Core.Services;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public interface INopStationPluginManager
    {
        Task<IList<INopStationPlugin>> LoadNopStationPluginsAsync(Customer customer = null, string pluginSystemName = "", 
            int storeId = 0, string widgetZone = null);

        Task<IPagedList<KeyValuePair<string, string>>> LoadPluginStringResourcesAsync(string pluginSystemName = "", 
            string keyword = "", int languageId = 0, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}