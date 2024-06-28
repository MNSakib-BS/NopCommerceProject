using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Plugin.Arch.Core.Domains.Payments;

namespace Nop.Plugin.Arch.Core.Services.Payments;
public partial interface ICustomerWalletService
{
    /// <summary>
    /// Validate the last modified date has not changed
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="storeId">Store id</param>
    /// <returns>True if wallet has not been modified; Otherwise, False</returns>
    Task<bool> HasCustomerWalletBeenModifiedAsync(int customerId, ShoppingCartType shoppingCartType = ShoppingCartType.ShoppingCart, int storeId = 0);

    /// <summary>
    /// Checks that the wallet exists
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="storeId">Store id</param>
    /// <returns>True if wallet exists; Otherwise, False</returns>
    Task<bool> HasWalletAsync(int customerId, int storeId = 0);

    /// <summary>
    /// Ensures the amount deducted from the wallet is valid.
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="storeId">Store id</param>
    Task<decimal> EnsureWalletDeductionAmountIsValidAsync(int customerId, int storeId = 0);

    Task<CustomerWallet> EnsureCustomerWalletAsync(int customerId, int storeId);

    Task DeleteCustomerWalletAsync(CustomerWallet wallet);

    Task<CustomerWallet> GetCustomerWalletByIdAsync(int customerWalletId);

    Task<decimal> GetAmountExcludingCashbackAsync(CustomerWallet customerWallet, int storeId);

    Task<IList<CustomerWallet>> GetAllCustomerWalletsAsync(int customerId,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0);

    Task<IPagedList<CustomerWallet>> SearchCustomerWalletsAsync(int? customerId,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

    Task<IPagedList<CustomerWalletTransaction>> SearchCustomerWalletTransactionsAsync(int customerWalletId,
        List<int> cashbackStatusIds, List<int> transactionTypeIds,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

    Task InsertCustomerWalletAsync(CustomerWallet wallet);

    Task UpdateCustomerWalletAsync(CustomerWallet wallet);

    Task<decimal> DeductCustomerWalletForOrderAsync(Order order);

    Task DeleteCustomerWalletTransactionAsync(CustomerWalletTransaction walletTransaction);

    Task<CustomerWalletTransaction> GetCustomerWalletTransactionByIdAsync(int customerWalletTransactionId);

    Task<IList<CustomerWalletTransaction>> GetAllCustomerWalletTransactionsAsync(int? customerWalletId = null,
        WalletTransactionType? transactionType = null,
        decimal? transactionTrackingNumber = null, bool? verifiedToArch = null,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0,
        CashbackStatus? cashbackStatus = null);

    Task InsertCustomerWalletTransactionAsync(CustomerWalletTransaction walletTransaction);

    Task UpdateCustomerWalletTransactionAsync(CustomerWalletTransaction walletTransaction);

    Task<bool> IsRefundCapturedAsync(int customerId, int storeId, int? cashbackType, decimal? transactionTrackingNumber = null);

    Task<bool> ClaimCashbackAsync(int customerWalletTransactionId,
        long LastModifiedTicks,
        out int? customerWalletId);

    Task<bool> VoidCashbackAsync(int customerWalletTransactionId,
        long LastModifiedTicks,
        out int? customerWalletId);

    Task ArchRefundCustomerWalletAsync(int customerId,
        decimal amount,
        string description,
        WalletTransactionType transactionType,
        int storeId,
        int? refundType,
        Order order,
        decimal? transactionTrackingNumber = null,
        CashbackStatus? cashbackStatus = null);
}

