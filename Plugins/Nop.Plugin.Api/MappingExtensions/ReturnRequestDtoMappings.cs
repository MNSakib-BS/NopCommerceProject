using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs.ReturnRequests;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ReturnRequestDtoMappings
    {
        public static ReturnRequestDto ToDto(this ReturnRequest returnRequest)
        {
            return returnRequest.MapTo<ReturnRequest, ReturnRequestDto>();
        }
    }
}
