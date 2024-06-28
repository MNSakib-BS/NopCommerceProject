using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain
{
    public class OrderShippingMethodCapacityMapping : BaseEntity
    {
        public int ShippingMethodCapacityId { get; set; }

        public int OrderId { get; set; }

        public DateTime DeliveryDateOnUtc { get; set; }
    }
}
