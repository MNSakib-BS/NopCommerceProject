using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class TimeSlotModel
    {
        public int AvailableDeliveryTimeRangeId { get; set; }

        public string Time {  get; set; }

        public int Capacity { get; set; }

        public int DayOfTheWeek { get; set; }

        public int ShippingMethodCapacityId { get; set; }

        public int ShippingMethodId { get; set; }

        public int AvailableDeliveryDateTimeRangeId { get; set; }

        public int DayOfMonth { get; set; }

    }
}
