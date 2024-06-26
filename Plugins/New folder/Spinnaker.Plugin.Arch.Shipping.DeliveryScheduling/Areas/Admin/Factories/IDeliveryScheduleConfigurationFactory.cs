using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Factories
{
    public interface IDeliveryScheduleConfigurationFactory
    {
        Task<List<SelectListItem>> PrepareAvailableShippingMethodsAsync();

        Task<AvailableDeliveryTimeRangeModel> PrepareModelAsync(AvailableDeliveryTimeRangeModel model, AvailableDeliveryTimeRange entity);

        Task<ShippingMethodAvailableDateCapacity> PrepareShippingMethodCapacityModelAsync(ShippingMethodAvailableDateCapacity model, int shippingMethodId = 0);

        Task<TimeRangeListModel> PrepareTimeRangeListModelAsync(TimeRangeSearchModel searchModel);

        Task<TimeRangeSearchModel> PrepareTimeRangeSearchModelAsync(TimeRangeSearchModel searchModel);

    }
}
