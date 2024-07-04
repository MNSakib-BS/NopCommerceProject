using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Orders
{
    [JsonObject(Title = "summary")]
    public class OrdersForDriverSummaryDto
    {
        [JsonProperty("new_orders_count")]
        public int NewOrdersCount { get; set; }

        [JsonProperty("orders_for_collection_count")]
        public int OrdersForCollectionCount { get; set; }

        [JsonProperty("orders_to_be_delivered_count")]
        public int OrdersToBeDeliveredCount { get; set; }

        [JsonProperty("orders_with_returns")]
        public int OrdersWithReturnsCount { get; set; }


        [JsonProperty("new_orders_with_changes_count")]
        public int NewOrdersWithChangesCount { get; set; }

        [JsonProperty("orders_for_collection_with_changes_count")]
        public int OrdersForCollectionWithChangesCount { get; set; }

        [JsonProperty("orders_to_be_delivered_with_changes_count")]
        public int OrdersToBeDeliveredWithChangesCount { get; set; }
    }
}
