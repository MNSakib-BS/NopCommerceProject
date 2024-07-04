using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.Services
{
    public class OrderNoteService : IOrderNoteService
    {
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IEventPublisher _eventPublisher;

        public OrderNoteService(
            IRepository<OrderNote> orderNoteRepository,
            IEventPublisher eventPublisher)
        {
            _orderNoteRepository = orderNoteRepository;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Insert an order note
        /// </summary>
        /// <param name="orderNote">OrderNote</param>
        public virtual async Task InsertOrderNoteAsync(OrderNote orderNote)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            _orderNoteRepository.Insert(orderNote);

            //event notification
            _eventPublisher.EntityInserted(orderNote);
        }

        public async Task<IList<OrderNote>> GetOrderNotesAsync(
            int? orderId, 
            DateTime? minDateUtc = null, 
            DateTime? maxDateUtc = null,
            int? orderNoteTypeId = null,
            int? createdByCustomerId = null,
            bool includeAcknowledged = true)
        {
            var orderNotes = _orderNoteRepository.Table;

            if (orderId.HasValue)
                orderNotes = orderNotes.Where(i => i.OrderId == orderId.Value);

            if (minDateUtc.HasValue)
                orderNotes = orderNotes.Where(i => i.CreatedOnUtc >= minDateUtc);

            if (maxDateUtc.HasValue)
                orderNotes = orderNotes.Where(i => i.CreatedOnUtc <= maxDateUtc);

            if (createdByCustomerId.HasValue)
                orderNotes = orderNotes.Where(i => i.CreatedByCustomerId == createdByCustomerId.Value);

            if (includeAcknowledged)
            {
                return !orderNoteTypeId.HasValue ? orderNotes.ToList() : orderNotes.Where(i => i.TypeId == orderNoteTypeId.Value).ToList();
            }
            else
            {
                var changeOrderNotes = new List<OrderNote>();

                foreach (var changeNote in orderNotes.Where(i => i.TypeId == (int)OrderNoteType.Change).ToList())
                {
                    var changeResponses = orderNotes.Where(i => i.TypeId == (int)OrderNoteType.ChangeResponse &&
                                                                i.ParentId.HasValue && 
                                                                i.ParentId == changeNote.Id).ToList();

                    var isAcknowledged = changeResponses.Any(i => i.Note == OrderNoteDefault.ChangeAcknowledged);
                    if (!isAcknowledged)
                    {      
                        changeOrderNotes.Add(changeNote);
                    }
                }

                return changeOrderNotes;
            }
        }
    }
}
