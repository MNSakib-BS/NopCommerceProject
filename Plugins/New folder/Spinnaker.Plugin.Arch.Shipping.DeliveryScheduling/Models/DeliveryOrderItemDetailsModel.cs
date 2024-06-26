using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public record DeliveryOrderItemDetailsModel : BaseNopEntityModel
    {
        public int CartId { get; set; }

        public int ShoppingCartItemId { get; set; }

        public bool Deleted { get; set; }
    }
}
