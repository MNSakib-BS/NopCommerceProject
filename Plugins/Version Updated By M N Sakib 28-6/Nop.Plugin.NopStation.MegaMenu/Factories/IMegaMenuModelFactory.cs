using Nop.Plugin.NopStation.MegaMenu.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.MegaMenu.Factories
{
    public interface IMegaMenuModelFactory
    {
        Task<MegaMenuModel> PrepareMegaMenuModelAsync();
    }
}
