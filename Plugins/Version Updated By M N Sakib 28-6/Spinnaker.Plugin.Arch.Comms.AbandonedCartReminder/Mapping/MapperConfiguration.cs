using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Arch.Core.Models.AbandonedCartReminder;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Mapping;
public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public int Order => 100;
    public MapperConfiguration()
    {
        CreateMap<Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder, AbandonedCartReminderModel>();
        CreateMap<AbandonedCartReminderModel, Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder>();
    }
}
