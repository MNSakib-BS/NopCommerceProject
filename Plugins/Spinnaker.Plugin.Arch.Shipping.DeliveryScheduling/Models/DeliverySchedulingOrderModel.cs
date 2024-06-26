using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class DeliverySchedulingOrderModel
    {
        [NopResourceDisplayName("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate")]
        public string DeliveryTimeRange { get; set; }

        [NopResourceDisplayName("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime")]
        public string DeliveryTime { get; set; }

        [NopResourceDisplayName("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.ShippingMethodName")]
        public string ShippingMethodName { get; set; }
    }
}
