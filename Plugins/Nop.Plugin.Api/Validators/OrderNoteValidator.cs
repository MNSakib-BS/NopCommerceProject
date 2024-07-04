using Nop.Plugin.Api.DTOs.OrderNotes;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class OrderNoteValidator : BaseDtoValidator<OrderNoteDto>
    {
        public OrderNoteValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
            : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetOrderIdRule();
        }

        private void SetOrderIdRule()
        {
            SetGreaterThanZeroRule(i => i.OrderId, "invalid order id");
        }
    }
}
