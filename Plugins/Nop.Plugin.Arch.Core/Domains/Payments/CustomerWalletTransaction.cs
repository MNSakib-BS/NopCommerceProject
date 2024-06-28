using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
public class CustomerWalletTransaction : BaseEntity, IStoreMappingSupported
{
    public int CustomerWalletId { get; set; }

    public int? OrderId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDateUtc { get; set; }

    public DateTime ModifiedDateUtc { get; set; }

    public int? ArchRefundType { get; set; }

    public decimal? TransactionTrackingNumber { get; set; }

    public int CashbackStatusId { get; set; }

    public CashbackStatus CashbackStatus
    {
        get => (CashbackStatus)CashbackStatusId;
        set => CashbackStatusId = (int)value;
    }

    public int WalletTransactionTypeId { get; set; }

    public WalletTransactionType WalletTransactionType
    {
        get => (WalletTransactionType)WalletTransactionTypeId;
        set => WalletTransactionTypeId = (int)value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    public int? StoreId { get; set; }
}
