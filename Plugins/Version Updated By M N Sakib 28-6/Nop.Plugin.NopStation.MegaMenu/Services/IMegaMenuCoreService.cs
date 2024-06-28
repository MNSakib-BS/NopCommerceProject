using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.NopStation.MegaMenu.Services
{
    public interface IMegaMenuCoreService
    {
        Task<IList<Category>> GetCategoriesByIdsAsync(int storeId = 0, List<int> selectedIds = null, int pageSize = int.MaxValue, bool showHidden = false, int storeTypeId = 0);

        Task<IList<Manufacturer>> GetManufacturersByIdsAsync(int storeId = 0, List<int> selectedIds = null, int pageSize = int.MaxValue, bool showHidden = false);
    }
}
