using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Customers;

namespace Nop.Plugin.Api.DTO.Drivers
{
    [JsonObject(Title = "driver")]
    public class DriverDto : CustomerDto
    {
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("vehicle_registration_number")]
        public string VehicleRegistrationNumber { get; set; }

        [JsonProperty("drivers_licence_number")]
        public string DriversLicenceNumber { get; set; }
    }
}
