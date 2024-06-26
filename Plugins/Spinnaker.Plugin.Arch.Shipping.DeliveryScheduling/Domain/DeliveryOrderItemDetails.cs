using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain
{
    public class DeliveryOrderItemDetails : BaseEntity
    {
        public int CartId { get; set; }

        public int ShoppingCartItemId { get; set; }

        public bool Deleted { get; set; }
    }
}
