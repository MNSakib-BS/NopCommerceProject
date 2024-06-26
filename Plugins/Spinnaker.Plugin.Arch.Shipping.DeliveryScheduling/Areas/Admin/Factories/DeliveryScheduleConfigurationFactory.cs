using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Factories
{
    public class DeliveryScheduleConfigurationFactory : IDeliveryScheduleConfigurationFactory
    {
        private readonly IStoreContext _storeContext;
        private readonly ITimeDeliveryService _timeDeliveryService;
        private readonly IShippingMethodService _shippingMethodService;

        public async Task<AvailableDeliveryTimeRangeModel> PrepareModelAsync(AvailableDeliveryTimeRangeModel model, AvailableDeliveryTimeRange entity)
        {
            if (model == null)
                model = new AvailableDeliveryTimeRangeModel();

            if (entity != null)
                model = entity.ToModel<AvailableDeliveryTimeRangeModel>();

            model.AvailableDeliveryDateTimeRangeModel = await PrepareAvailableTimeRangeAsync();

            return model;
        }

        public async Task<ShippingMethodAvailableDateCapacity> PrepareShippingMethodCapacityModelAsync(ShippingMethodAvailableDateCapacity model, int shippingMethodId = 0)
        {
            if (model == null)
                model = new ShippingMethodAvailableDateCapacity();

            foreach (var day in Enum.GetValues(typeof(DeliveryDayEnum)))
            {
                model.Days.TryAdd(day.ToString(), (int)day);
            }

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var availableDeliveryTimeRanges = await _timeDeliveryService.GetAllDeliveryTimeRangeAsync(storeScope);

            foreach (var timeRange in availableDeliveryTimeRanges)
            {
                var availavleDeliveryDateTimeRange = await _timeDeliveryService.GetAllDeliveryDateTimeRangeAsync(timeRange.Id);

                foreach (var deliveryDateTimeRange in availavleDeliveryDateTimeRange)
                {
                    var shippingMethodCapacity = (await _shippingMethodService.GetAllShippingMethodsAsync(deliveryDateTimeRange.Id, shippingMethodId)).FirstOrDefault();

                    if (shippingMethodCapacity != null)
                        model.Capacities.Add(shippingMethodCapacity.Id, (shippingMethodCapacity.Capacity, deliveryDateTimeRange.DayOfWeek));
                }

                model.TimeRange.Add(timeRange.Time);
            }
            return model;
        }

        public async Task<TimeRangeSearchModel> PrepareTimeRangeSearchModelAsync(TimeRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<TimeRangeListModel> PrepareTimeRangeListModelAsync(TimeRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var timeRanges = await _timeDeliveryService.GetAllDeliveryTimeRangeAsync(storeScope);

            //prepare list model
            var model = new TimeRangeListModel().PrepareToGrid(searchModel, timeRanges, () =>
            {
                return timeRanges.Select(timeRange =>
                {
                    //fill in model values from the entity
                    var timeRangemodel = timeRange.ToModel<AvailableDeliveryTimeRangeModel>();

                    return timeRangemodel;
                });
            });

            return model;
        }

        public async Task<List<SelectListItem>> PrepareAvailableShippingMethodsAsync()
        {
            var availableShippingMethods = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Text = "Select...",
                    Value = "0"
                }
            };

            var shippingMethods = await _shippingMethodService.GetAllAsync();

            foreach (var item in shippingMethods)
            {
                availableShippingMethods.Add(new SelectListItem
                {
                    Text = item.Name.ToString(),
                    Value = item.Id.ToString()
                });
            }

            return availableShippingMethods;
        }

        private async Task<List<SelectListItem>> PrepareAvailableTimeRangeAsync()
        {
            var availableTimeRange = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Text = "Select...",
                    Value = "0"
                }
            };

            foreach (TimeRangeEnum timeRange in Enum.GetValues(typeof(TimeRangeEnum)))
            {
                var memberInfo = typeof(TimeRangeEnum).GetMember(timeRange.ToString());
                var descriptionAttribute = (DescriptionAttribute)memberInfo[0].GetCustomAttribute(typeof(DescriptionAttribute), false);
                var description = descriptionAttribute?.Description ?? timeRange.ToString();

                availableTimeRange.Add(new SelectListItem
                {
                    Text = description,
                    Value = ((int)timeRange).ToString()
                });
            }

            return availableTimeRange;
        }
    }
}
