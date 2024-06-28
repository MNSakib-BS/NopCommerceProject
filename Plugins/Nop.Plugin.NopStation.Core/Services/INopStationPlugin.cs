using System.Collections.Generic;
using Nop.Services.Plugins;

namespace Nop.Plugin.NopStation.Core.Services
{
    public interface INopStationPlugin : IPlugin
    {
        List<KeyValuePair<string, string>> PluginResouces();
    }
}
