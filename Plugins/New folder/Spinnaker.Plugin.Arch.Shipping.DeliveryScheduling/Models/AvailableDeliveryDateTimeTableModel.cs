using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public record AvailableDeliveryDateTimeRangeModel : BaseNopEntityModel
    {
        public int? ExceptionDateId { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }

        public int DayOfWeek { get; set; }

        public DateTime StartDateOnUtc { get; set; }

        public DateTime EndDateOnUtc { get; set; }
    }
}
