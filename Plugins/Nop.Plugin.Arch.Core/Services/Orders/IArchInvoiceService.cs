using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Arch.Core.Domains.Orders;

namespace Nop.Plugin.Arch.Core.Services.Orders;
public interface IArchInvoiceService
{
    Task InsertInvoiceItemAsync(ArchInvoiceItem archInvoiceItem);
    Task<IList<ArchInvoiceItem>> GetInvoiceItemsAsync(decimal transactionTrackingNumber, int storeId);
    //void InsertQuotationItem(int customerID, int storeID, decimal transactionTrackingNumber);
    //decimal GetTrackingNumberFromOrder(int customerID, int storeID);
    //void DeleteQuotationItem(decimal transactionTrackingNumber, int storeID);
    Task ConfirmArchAsync(Order order, Customer customer, int storeId, DateTime? deliveryDateTime);
}
