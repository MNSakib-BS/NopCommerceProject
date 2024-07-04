using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class OrderStatusUpdateDtoValidator : BaseDtoValidator<OrderStatusUpdateDto>
    {
        public OrderStatusUpdateDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
            : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetOrderIdRule();
        }

        private void SetOrderIdRule()
        {
            SetGreaterThanZeroRule(i => i.Id, "valid id required");
        }
    }
}
