using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Nop.Web.Framework.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Models
{
    public record TimeRangeListModel : BasePagedListModel<AvailableDeliveryTimeRangeModel>
    {
    }
}
