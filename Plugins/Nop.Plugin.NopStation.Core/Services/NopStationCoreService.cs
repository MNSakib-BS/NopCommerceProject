using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.NopStation.Core.Services
{
    public class NopStationCoreService : INopStationCoreService
    {
        private readonly ILocalizationService _localizationService;

        public NopStationCoreService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode, SiteMapNode childNode, NopStationMenuType menuType = NopStationMenuType.Root)
        {
            var nopstationNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "NopStation");
            if (nopstationNode == null)
            {
                nopstationNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.NopStation"),
                    SystemName = "NopStation",
                    IconClass = "fa-folder-open"
                };
                rootNode.ChildNodes.Add(nopstationNode);
            }

            var pluginNode = nopstationNode.ChildNodes.FirstOrDefault(x => x.SystemName == "NopStationPlugin");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.Plugins"),
                    SystemName = "NopStationPlugin",
                    IconClass = "fa-plug"
                };
                nopstationNode.ChildNodes.Add(pluginNode);
            }

            var themeNode = nopstationNode.ChildNodes.FirstOrDefault(x => x.SystemName == "NopStationTheme");
            if (themeNode == null)
            {
                themeNode = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.Themes"),
                    SystemName = "NopStationTheme",
                    IconClass = "fa-picture-o"
                };
                nopstationNode.ChildNodes.Add(themeNode);
            }

            var coreNode = nopstationNode.ChildNodes.FirstOrDefault(x => x.SystemName == "NopStationCore");
            if (coreNode == null)
            {
                coreNode = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Core.Menu.Core"),
                    SystemName = "NopStationCore",
                    IconClass = "fa-wrench"
                };
                nopstationNode.ChildNodes.Add(coreNode);
            }

            if (menuType == NopStationMenuType.Plugin)
            {
                pluginNode.Visible = true;
                pluginNode.ChildNodes.Add(childNode);
            }
            else if (menuType == NopStationMenuType.Core)
            {
                coreNode.Visible = true;
                coreNode.ChildNodes.Add(childNode);
            }
            else if (menuType == NopStationMenuType.Theme)
            {
                themeNode.Visible = true;
                themeNode.ChildNodes.Add(childNode);
            }
            else if (menuType == NopStationMenuType.Root)
            {
                nopstationNode.ChildNodes.Add(childNode);
            }
            else
            {
                rootNode.ChildNodes.Add(childNode);
            }
        }
    }
}
