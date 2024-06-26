using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services
{
    public interface ITimeDeliveryService
    {
        Task DeleteTimeRangeAsync(AvailableDeliveryTimeRange timeRange);
        Task<IPagedList<AvailableDeliveryTimeRange>> GetAllDeliveryTimeRangeAsync(int storeId = 0, int id = 0, bool deleted = false, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<IQueryable<AvailableDeliveryDateTimeRange>> GetAllDeliveryDateTimeRangeAsync(int availableDeliveryTimeRangeId = 0, int id = 0);
        Task<AvailableDeliveryTimeRange> GetByIdAsync(int id);
        Task InsertDateTimeRangeAsync(AvailableDeliveryDateTimeRange dateTimeRange);
        Task InsertTimeRangeAsync(AvailableDeliveryTimeRange timeRange);
        Task UpdateTimeRangeAsync(AvailableDeliveryTimeRange timeRange);
    }
}