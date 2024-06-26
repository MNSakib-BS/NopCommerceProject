using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain
{
    public class ShippingMethodCapacity : BaseEntity
    {
        public int AvailableDeliveryDateTimeRangeId { get; set; }

        public int ShippingMethodId { get; set; }

        public int Capacity { get; set; }

        public bool Deleted { get; set; }
    }
}
