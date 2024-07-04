using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Orders
{
    public class OrdersSummaryRootObject
    {
        [JsonProperty("summary")]
        public OrdersForDriverSummaryDto Summary { get; set; }
    }
}
