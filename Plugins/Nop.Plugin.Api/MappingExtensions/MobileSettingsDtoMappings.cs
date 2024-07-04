using Nop.Core.Domain.Mobile;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.Mobile;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class MobileSettingsDtoMappings
    {
        public static MobileSettingsDto ToDto(this MobileSettings settings)
        {
            return settings.MapTo<MobileSettings, MobileSettingsDto>();
        }
    }
}
