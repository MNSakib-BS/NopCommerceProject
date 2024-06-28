using AutoMapper;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.Core.Models;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 1;

        public MapperConfiguration()
        {
            CreateMap<CoreLocaleResourceModel, LocaleStringResource>()
                .ForMember(entity => entity.LanguageId, options => options.Ignore());
        }
    }
}
