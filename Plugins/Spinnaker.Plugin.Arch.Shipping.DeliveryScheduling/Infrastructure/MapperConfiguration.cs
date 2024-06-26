using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models;
using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Models;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public MapperConfiguration()
        {
            CreateMap<AvailableDeliveryTimeRange, AvailableDeliveryTimeRangeModel>();
            CreateMap<AvailableDeliveryTimeRangeModel, AvailableDeliveryTimeRange>();
        }
    }
}
