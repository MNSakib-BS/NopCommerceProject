using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain
{
    public class AvailableDeliveryTimeRange : BaseEntity
    {
        public string Time { get; set; }

        public int StoreId { get; set; }

        public int DisplayOrder { get; set; }

        public bool Deleted { get; set; }
    }
}
