using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.ReturnRequests;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class ReturnRequestValidator : BaseDtoValidator<ReturnRequestDto>
    {
        public ReturnRequestValidator(
            IHttpContextAccessor httpContextAccessor, 
            IJsonHelper jsonHelper, 
            Dictionary<string, object> requestJsonDictionary) : 
            base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetGreaterThanZeroRule(i => i.StoreId, "invalid store id");

            SetGreaterThanZeroRule(i => i.OrderItemId, "invalid order item id");

            SetGreaterThanZeroRule(i => i.CustomerId, "invalid customer id");

            SetGreaterThanZeroRule(i => i.Quantity, "invalid quantity id");
        }
    }
}
