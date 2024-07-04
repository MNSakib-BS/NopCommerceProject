using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.ReturnRequests;

namespace Nop.Plugin.Api.DTO.ReturnRequests
{
    public class ReturnRequestsRootObject : ISerializableObject
    {
        public ReturnRequestsRootObject()
        {
            ReturnRequests = new List<ReturnRequestDto>();
        }

        [JsonProperty("return_requests")]
        public IList<ReturnRequestDto> ReturnRequests { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "return_requests";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ReturnRequestDto);
        }
    }
}
