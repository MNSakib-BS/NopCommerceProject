using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public record AvailableDeliveryDateTimeModel : BaseNopEntityModel
    {
        public AvailableDeliveryDateTimeModel()
        {
            AvailableDeliveryTimeRangeModel = new AvailableDeliveryTimeRangeModel();
            NextAvailableDeliveryDates = new Dictionary<string, int>();
            DeliveryDates = new List<DeliveryDateModel>();
        }

        public AvailableDeliveryTimeRangeModel AvailableDeliveryTimeRangeModel { get; set; }

        public ShippingMethodAvailableCapacity ShippingMethodAvailableCapacity { get; set; }

        public Dictionary<string, int> NextAvailableDeliveryDates { get; set; }

        public IList<DeliveryDateModel> DeliveryDates { get; set; }

    }
    public class DeliveryDateModel
    {
        public DeliveryDateModel()
        {
            TimeSlots = new List<TimeSlotModel>();
        }
        public int DayOfMonth { get; set; }
        public string Date { get; set; }
        public int DayOfWeek { get; set; }
        public IList<TimeSlotModel> TimeSlots { get; set; }
    }

}
