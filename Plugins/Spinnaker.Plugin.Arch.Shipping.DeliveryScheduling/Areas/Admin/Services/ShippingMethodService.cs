using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LinqToDB.Reflection.Methods.LinqToDB;
using ShippingMethodCapacity = Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain.ShippingMethodCapacity;
using Nop.Core.Events;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services
{
    public class ShippingMethodService : IShippingMethodService
    {
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly IRepository<ShippingMethodCapacity> _shippingMethodCapacityRepository;
        private readonly IEventPublisher _eventPublisher;

        public ShippingMethodService(IRepository<ShippingMethod> shippingMethodRepository, IEventPublisher eventPublisher, IRepository<ShippingMethodCapacity> shippingMethodCapacityRepository)
        {
            _shippingMethodRepository = shippingMethodRepository;
            _eventPublisher = eventPublisher;
            _shippingMethodCapacityRepository = shippingMethodCapacityRepository;
        }

        public async Task<IQueryable<ShippingMethod>> GetAllAsync(int id = 0, string name = "")
        {
            var shippingMethodQuery = _shippingMethodRepository.Table;

            if (id > 0)
                shippingMethodQuery = shippingMethodQuery.Where(i => i.Id == id);

            if (!string.IsNullOrEmpty(name))
                shippingMethodQuery = shippingMethodQuery.Where(i => i.Name == name);

            return await Task.FromResult(shippingMethodQuery);
        }

        public async Task<ShippingMethodCapacity> GetShippingCapacityByIdAsync(int id)
        {
            var shippingMethodQuery = _shippingMethodCapacityRepository.Table;
            var shippingMethod = await shippingMethodQuery.FirstOrDefaultAsync(i => i.Id == id);
            return shippingMethod;
        }

        public async Task<IQueryable<ShippingMethodCapacity>> GetAllShippingMethodsAsync(int availableDeliveryDateTimeRangeId = 0, int shippingMethodId = 0, bool deleted = false)
        {
            var query = _shippingMethodCapacityRepository.Table;

            if (!deleted)
                query = query.Where(i => i.Deleted == deleted);

            if (shippingMethodId > 0)
                query = query.Where(i => i.ShippingMethodId == shippingMethodId);

            if (availableDeliveryDateTimeRangeId > 0)
                query = query.Where(i => i.AvailableDeliveryDateTimeRangeId == availableDeliveryDateTimeRangeId);

            return await Task.FromResult(query);
        }

        public async Task<IQueryable<ShippingMethodCapacity>> GetShippingMethodCapacityByShippingMethodIdAsync(int shippingMethodId, bool deleted = false)
        {
            var shippingMethodQuery = _shippingMethodCapacityRepository.Table;
            shippingMethodQuery = shippingMethodQuery.Where(i => i.ShippingMethodId == shippingMethodId && i.Deleted == deleted && i.Capacity > 0);
            return await Task.FromResult(shippingMethodQuery);
        }

        public async Task InsertShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                throw new ArgumentNullException(nameof(shippingMethodCapacity));

            await _shippingMethodCapacityRepository.InsertAsync(shippingMethodCapacity);
            await _eventPublisher.EntityInsertedAsync(shippingMethodCapacity);
        }

        public async Task UpdateShippingMethodCapacityAsync(ShippingMethodCapacity shippingMethodCapacity)
        {
            if (shippingMethodCapacity == null)
                throw new ArgumentNullException(nameof(shippingMethodCapacity));

            await _shippingMethodCapacityRepository.UpdateAsync(shippingMethodCapacity);
            await _eventPublisher.EntityUpdatedAsync(shippingMethodCapacity);
        }

        public async Task DeleteShippingMethodCapacityByDeliveryDateIdAsync(int availableDeliveryDateTimeRangeId)
        {
            var shippingMethodQuery = _shippingMethodCapacityRepository.Table;
            var shippingMethods = shippingMethodQuery.Where(i => i.AvailableDeliveryDateTimeRangeId == availableDeliveryDateTimeRangeId);

            foreach (var item in shippingMethods)
            {
                item.Deleted = true;
                await _shippingMethodCapacityRepository.UpdateAsync(item);
                await _eventPublisher.EntityUpdatedAsync(item);
            }
        }

    }
}
