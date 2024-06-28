using System.Threading.Tasks;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.NopStation.Core.Services
{
    public interface INopStationCoreService
    {
        Task ManageSiteMapAsync(SiteMapNode rootNode, SiteMapNode childNode, NopStationMenuType menuType = NopStationMenuType.Root);
    }
}