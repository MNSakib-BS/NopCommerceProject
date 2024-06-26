using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Services;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Services;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Factories
{
    public class DeliveryScheduleFactory : IDeliveryScheduleFactory
    {
        private readonly IShippingMethodService _shippingMethodService;
        private readonly ITimeDeliveryService _timeDeliveryService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;

        public DeliveryScheduleFactory(IShippingMethodService shippingMethodService,
                                       ITimeDeliveryService timeDeliveryService,
                                       ISettingService settingService,
                                       IStoreContext storeContext,
                                       IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService)
        {
            _shippingMethodService = shippingMethodService;
            _timeDeliveryService = timeDeliveryService;
            _settingService = settingService;
            _storeContext = storeContext;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
        }

        private async Task<Dictionary<Tuple<DateTime, int>, int>> GetPlacedOrdersCountsAsync(DateTime startDate, int daysRange)
        {
            var placedOrdersCounts = await _orderShippingMethodCapacityMappingService.GetPlacedOrdersCountsForAllAsync(startDate, daysRange);
            return placedOrdersCounts;
        }

        private async Task< Dictionary<DateTime, int> >GetAvailableDatesAsync()
        {
            Dictionary<DateTime, int> dateList = new Dictionary<DateTime, int>();
            DateTime currentDate = DateTime.Now;

            var storeScope = (await _storeContext.GetCurrentStoreAsync()).Id;
            var settings = _settingService.LoadSetting<DeliverySchedulingSettings>(storeScope);
            var dayOffset = settings.DayOffset;

            for (int i = 0; i < dayOffset + 1; i++)
            {
                var date = currentDate.AddDays(i);
                var enumValue = (int)Enum.Parse(typeof(DeliveryDayEnum), date.DayOfWeek.ToString());
                dateList.Add(date, enumValue);
            }

            return dateList;
        }

        public async Task<AvailableDeliveryDateTimeModel> PrepareModelAsync(AvailableDeliveryDateTimeModel model, AvailableDeliveryTimeRange entity)
        {
            if (model == null)
                model = new AvailableDeliveryDateTimeModel();

            var nextSevenDates = GetAvailableDatesAsync();
            var startDate = nextSevenDates.Keys.Min();
            var endDate = nextSevenDates.Keys.Max();
            var daysRange = (endDate - startDate).Days;

            var shippingMethod = (await _shippingMethodService.GetAllAsync(name: "Delivery")).FirstOrDefault();
            if (shippingMethod == null)
                throw new InvalidOperationException("Shipping method not found.");

            var shippingMethodCapacities =( await _shippingMethodService.GetShippingMethodCapacityByShippingMethodIdAsync(shippingMethod.Id)).ToList();
            var allDeliveryDateTimeRanges =(await _timeDeliveryService.GetAllDeliveryDateTimeRangeAsync()).ToList();
            var allTimeSlots =(await _timeDeliveryService.GetAllDeliveryTimeRangeAsync((await _storeContext.GetCurrentStoreAsync()).Id)).ToList();

            var placedOrdersCounts = await GetPlacedOrdersCountsAsync(startDate, daysRange);

            var dateTimeRangesDict = allDeliveryDateTimeRanges.ToDictionary(r => r.Id);

            var timeSlotsDict = allTimeSlots.GroupBy(ts => ts.Id)
                                            .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var dateEntry in await nextSevenDates)
            {
                var date = dateEntry.Key;
                var deliveryDateModel = new DeliveryDateModel
                {
                    Date = date.ToString("dddd, MMMM dd, yyyy"),
                    DayOfWeek = dateEntry.Value,
                    DayOfMonth = date.Day
                };

                foreach (var capacity in shippingMethodCapacities)
                {
                    var key = Tuple.Create(date.Date, capacity.Id);
                    int placedOrdersCount = placedOrdersCounts.ContainsKey(key) ? placedOrdersCounts[key] : 0;

                    if (placedOrdersCount >= capacity.Capacity)
                        continue;

                    if (dateTimeRangesDict.TryGetValue(capacity.AvailableDeliveryDateTimeRangeId, out var deliveryDateTimeRange) &&
                        deliveryDateTimeRange.DayOfWeek == deliveryDateModel.DayOfWeek)
                    {
                        if (timeSlotsDict.TryGetValue(deliveryDateTimeRange.AvailableDeliveryTimeRangeId, out var timeSlotsList))
                        {
                            foreach (var timeSlot in timeSlotsList)
                            {
                                if (await IsTimeSlotWithinHourOffsetAsync(timeSlot.Time, date))
                                {
                                    deliveryDateModel.TimeSlots.Add(new TimeSlotModel
                                    {
                                        DayOfTheWeek = deliveryDateTimeRange.DayOfWeek,
                                        AvailableDeliveryTimeRangeId = deliveryDateTimeRange.Id,
                                        Time = timeSlot.Time,
                                        Capacity = capacity.Capacity,
                                        ShippingMethodCapacityId = capacity.Id,
                                        ShippingMethodId = shippingMethod.Id,
                                        AvailableDeliveryDateTimeRangeId = deliveryDateTimeRange.Id,
                                        DayOfMonth = date.Day
                                    });
                                }
                            }
                        }
                    }
                }

                if (deliveryDateModel.TimeSlots.Count > 0)
                    model.DeliveryDates.Add(deliveryDateModel);
            }

            return model;
        }

        private async Task<bool> IsTimeSlotWithinHourOffsetAsync(string timeSlot, DateTime currentDateTime)
        {
            var startTimeStr = timeSlot.Split('-')[0].Trim();

            if (TimeSpan.TryParseExact(startTimeStr, "hh'h'mm", CultureInfo.InvariantCulture, out var startTime))
            {
                if (currentDateTime.Date == DateTime.Now.Date)
                {
                    var timeSlotDateTime = currentDateTime.Date + startTime;

                    var timeDifference = timeSlotDateTime.TimeOfDay - DateTime.Now.TimeOfDay;
                    var storeScope = (await _storeContext.GetCurrentStoreAsync()).Id; // Ensure this method is async
                    var settings = await _settingService.LoadSettingAsync<DeliverySchedulingSettings>(storeScope); // Ensure this method is async

                    return timeDifference >= TimeSpan.FromMinutes(0) && timeDifference >= TimeSpan.FromHours(settings.HourOffset);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

    }
}
