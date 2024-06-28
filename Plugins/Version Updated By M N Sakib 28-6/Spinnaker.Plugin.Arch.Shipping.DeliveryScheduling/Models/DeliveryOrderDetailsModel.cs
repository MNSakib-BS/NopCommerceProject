using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public record DeliveryOrderDetailsModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }

        public int StoreId { get; set; }

        public int OrderId { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }
    }
}
