using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class AvailableDeliveryTimesTableModel
    {
        public string Time { get; set; }

        public int ShippingMethodCapacityId { get; set; }

        public int AvailableCapacities { get; set; }

        public int AvailableDeliveryTimeRangeId { get; set; }
    }
}
