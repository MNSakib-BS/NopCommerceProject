using Newtonsoft.Json;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTOs.Orders
{
    [JsonObject(Title = "order_update")]
    public class OrderStatusUpdateDto : BaseDto
    {
        [JsonProperty("allocated_to_driver_id")]
        public int? AllocatedToDriverId { get; set; }

        [JsonProperty("shipping_status_id")]
        public int? ShippingStatusId { get; set; }

        public ShippingStatus? ShippingStatus
        {
            get => (ShippingStatus?)ShippingStatusId;
            set => ShippingStatusId = (int?)value;
        }
    }
}
