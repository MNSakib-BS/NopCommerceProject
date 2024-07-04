using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.OrderNotes;
using Nop.Plugin.Api.AutoMapper;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class OrderNoteDtoMappings
    {
        public static OrderNoteDto ToDto(this OrderNote orderNote)
        {
            return orderNote.MapTo<OrderNote, OrderNoteDto>();
        }
    }
}
