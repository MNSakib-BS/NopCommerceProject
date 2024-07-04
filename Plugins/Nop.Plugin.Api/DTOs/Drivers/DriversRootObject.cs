using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTO.Drivers
{
    public class DriversRootObject : ISerializableObject
    {
        public DriversRootObject()
        {
            Drivers = new List<DriverDto>();
        }

        [JsonProperty("drivers")]
        public IList<DriverDto> Drivers { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "drivers";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(DriverDto);
        }
    }
}
