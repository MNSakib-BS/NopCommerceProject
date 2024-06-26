using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain
{
    public class DeliveryOrderDetails : BaseEntity
    {
        public int CustomerId { get; set; }

        public int StoreId { get; set; }

        public int OrderId { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }
    }
}
