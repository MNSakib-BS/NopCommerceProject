using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Mobile;

namespace Nop.Plugin.Api.DTO.ReturnRequests
{
    public class MobileSettingsRootObject : ISerializableObject
    {
        public MobileSettingsRootObject()
        {
            MobileSettings = new MobileSettingsDto();
        }

        [JsonProperty("mobile_Settings")]
        public MobileSettingsDto MobileSettings { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "mobile_Settings";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(MobileSettingsDto);
        }
    }
}
