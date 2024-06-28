using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services
{
    public class ShippingMethodCapacityService : IShippingMethodCapacityService
    {
        private readonly IRepository<ShippingMethodCapacity> _shippingMethodCapacityRepo;

        public ShippingMethodCapacityService(IRepository<ShippingMethodCapacity> shippingMethodCapacityRepo)
        {
            _shippingMethodCapacityRepo = shippingMethodCapacityRepo;
        }

        public async Task SaveAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                return;

            await _shippingMethodCapacityRepo.InsertAsync(shippingMethodCapacity);
        }

        public async Task UpdateAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                return;

            await _shippingMethodCapacityRepo.UpdateAsync(shippingMethodCapacity);
        }

        public async Task<IPagedList<ShippingMethodCapacity>> GetShippingMethodCapacitiesAsync(
            int shippingMethodId = 0,
            int availableDeliveryDateTimeRangeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            IQueryable<ShippingMethodCapacity> source = _shippingMethodCapacityRepo.Table;

            if (shippingMethodId > 0)
                source = source.Where(w => w.ShippingMethodId == shippingMethodId);

            if (availableDeliveryDateTimeRangeId > 0)
                source = source.Where(w => w.AvailableDeliveryDateTimeRangeId == availableDeliveryDateTimeRangeId);

            source = source.Where(e => !e.Deleted);

            return await source.ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task DeleteAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                return;

            shippingMethodCapacity.Deleted = true;
            await _shippingMethodCapacityRepo.UpdateAsync(shippingMethodCapacity);
        }

        public async Task<ShippingMethodCapacity> GetShippingMethodCapacityAsync(int id)
        {
            return await _shippingMethodCapacityRepo.GetByIdAsync(id);
        }
    }
}
