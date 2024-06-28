using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class OrderDetailsDeliveryDateTimeModel
    {
        public string DeliveryDate { get; set; }

        public string DeliveryTime { get; set; }

        public bool IsOpc { get; set; }
    }
}
