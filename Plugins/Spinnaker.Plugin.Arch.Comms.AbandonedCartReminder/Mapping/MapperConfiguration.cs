using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Mapping;
public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public int Order => 100;
    public MapperConfiguration()
    {
        CreateMap<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder, AbandonedCartReminderModel>();
        CreateMap<AbandonedCartReminderModel, Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>();
    }
}
