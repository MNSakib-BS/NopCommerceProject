using Nop.Plugin.Api.DTO.Drivers;

namespace Nop.Plugin.Api.Services
{
    public interface IDriverApiService
    {
       Task< DriverDto> GetDriverByIdAsync(int id, bool showDeleted = false);
    }
}
