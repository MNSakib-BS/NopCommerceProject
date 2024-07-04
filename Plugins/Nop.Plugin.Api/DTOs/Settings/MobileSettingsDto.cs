using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.Mobile
{
    [JsonObject(Title = "mobile_settings")]
    public class MobileSettingsDto : BaseDto
    {
        [JsonProperty("google_api_key")]
        public string GoogleApiKey { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("primary_color")]
        public string PrimaryColor { get; set; }
        
        [JsonProperty("primary_light_color")]
        public string PrimaryLightColor { get; set; }
        
        [JsonProperty("secondary_color")]
        public string SecondaryColor { get; set; }
        
        [JsonProperty("secondary_light_color")]
        public string SecondaryLightColor { get; set; }
        
        [JsonProperty("error_color")]
        public string ErrorColor { get; set; }
        
        [JsonProperty("line_separator_color")]
        public string LineSeparatorColor { get; set; }
        
        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }
        
        [JsonProperty("admin_changes_color")]
        public string AdminChangesColor { get; set; }

        [JsonProperty("order_download_limit")]
        public int OrderDownloadLimit { get; set; }
    }
}
