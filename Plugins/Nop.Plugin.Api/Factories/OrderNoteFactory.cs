using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Api.Factories
{
    public class OrderNoteFactory : IFactory<OrderNote>
    {
        public async Task<OrderNote> InitializeAsync()
        {
            var defaultOrderNote = new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow
            };

            return defaultOrderNote;
        }

        
    }
}
