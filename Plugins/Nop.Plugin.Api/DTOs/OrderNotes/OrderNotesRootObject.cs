using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.OrderNotes;

namespace Nop.Plugin.Api.DTO.Orders
{
    public class OrderNotesRootObject : ISerializableObject
    {
        public OrderNotesRootObject()
        {
            OrderNotes = new List<OrderNoteDto>();
        }

        [JsonProperty("order_notes")]
        public IList<OrderNoteDto> OrderNotes { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "order_notes";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(OrderNoteDto);
        }
    }
}
