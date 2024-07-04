using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.Orders
{
    [JsonObject(Title = "orderSearch")]
    public class OrderSearchDto
    {
        [JsonProperty("driver_id")]
        public int? DriverId { get; set; }

        [JsonProperty("driver_email")]
        public string DriverEmail { get; set; }

        [JsonProperty("created_at_min")]
        public DateTime? CreatedAtMin { get; set; }

        [JsonProperty("created_at_max")]
        public DateTime? CreatedAtMax { get; set; }

        [JsonProperty("limit")]
        public int? Limit { get; set; }

        [JsonProperty("page")]
        public int? Page { get; set; }

        [JsonProperty("order_status_ids")]
        public ICollection<int> OrderStatusIds { get; set; }

        [JsonProperty("payment_status_ids")]
        public ICollection<int> PaymentStatusIds { get; set; }

        [JsonProperty("shipping_status_ids")]
        public ICollection<int> ShippingStatusIds { get; set; }

        [JsonProperty("order_note_type")]
        public int? OrderNoteType { get; set; }

        [JsonProperty("include_acknowledged_notes")]
        public bool? IncludeAcknowledgedNotes { get; set; }
    }
}
