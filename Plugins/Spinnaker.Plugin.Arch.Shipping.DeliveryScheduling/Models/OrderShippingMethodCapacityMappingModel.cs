using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public record OrderShippingMethodCapacityMappingModel : BaseNopEntityModel
    {
        public int ShippingMethodCapacityId { get; set; }

        public int OrderId { get; set; }

        public DateTime DeliveryDateOnUtc { get; set; }
    }
}
