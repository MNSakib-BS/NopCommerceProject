using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Arch.Core.Domains.Orders;

namespace Nop.Plugin.Arch.Core.Services.Orders;
public class ArchInvoiceService : IArchInvoiceService
{
    #region Fields

    #endregion

    #region Ctor

    #endregion

    #region Methods

    public virtual async Task ConfirmArchAsync(Order order, Customer customer, int storeId, DateTime? deliveryDateTime)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<IList<ArchInvoiceItem>> GetInvoiceItemsAsync(decimal transactionTrackingNumber, int storeId)
    {
        throw new NotImplementedException();
    }

    public virtual async Task InsertInvoiceItemAsync(ArchInvoiceItem archInvoiceItem)
    {
        throw new NotImplementedException();
    }

    #endregion


}
