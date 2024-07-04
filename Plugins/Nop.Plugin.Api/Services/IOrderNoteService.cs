using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderNoteService
    {
        Task InsertOrderNoteAsync(OrderNote orderNote);

        Task<IList<OrderNote>> GetOrderNotesAsync(
               int? orderId,
               DateTime? minDateUtc = null,
               DateTime? maxDateUtc = null,
               int? orderNoteTypeId = null,
               int? createdByCustomerId = null,
               bool includeAcknowledged = true);
    }
}
