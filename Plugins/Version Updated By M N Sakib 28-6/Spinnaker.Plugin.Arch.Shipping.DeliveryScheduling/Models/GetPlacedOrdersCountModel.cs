using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class GetPlacedOrdersCountModel
    {
        public int ShippingMethodId { get; set; }

        public DateTime DeliveryDateOnUtc { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }

        public int OrdersCount { get; set; }
    }
}
