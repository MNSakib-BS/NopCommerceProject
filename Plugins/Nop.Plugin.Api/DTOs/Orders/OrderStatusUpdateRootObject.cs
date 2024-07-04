using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Orders
{
    public class OrderStatusUpdateRootObject
    {
        [JsonProperty("order_update")]
        public OrderStatusUpdateDto OrderUpdate { get; set; }
    }
}
