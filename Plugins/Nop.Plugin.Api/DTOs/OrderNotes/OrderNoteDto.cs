using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;
using System;

namespace Nop.Plugin.Api.DTOs.OrderNotes
{
    [JsonObject(Title = "order_note")]
    public class OrderNoteDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the parent note identifier
        /// </summary>
        [JsonProperty("parent_id")]
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order note creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets an order note type identifier
        /// </summary>
        [JsonProperty("type_id")]
        public int TypeId { get; set; }

        /// <summary>
        /// Gets or sets the created by customer id
        /// </summary>
        [JsonProperty("customer_by_customer_id")]
        public int? CreatedByCustomerId { get; set; }

        /// <summary>
        /// Gets or sets an order note type identifier
        /// </summary>
        [JsonProperty("order_note_change_type_id")]
        public int? OrderNoteChangeTypeId { get; set; }
    }
}
