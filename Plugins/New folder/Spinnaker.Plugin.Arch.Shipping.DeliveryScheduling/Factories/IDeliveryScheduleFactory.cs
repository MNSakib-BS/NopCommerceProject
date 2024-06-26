using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Factories
{
    public interface IDeliveryScheduleFactory
    {
        Task<AvailableDeliveryDateTimeModel> PrepareModelAsync(AvailableDeliveryDateTimeModel model, AvailableDeliveryTimeRange entity);
    }
}