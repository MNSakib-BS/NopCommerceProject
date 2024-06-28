using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Payments;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.Payments;
/// <summary>
/// Customer wallet service
/// </summary>
public partial class CustomerWalletService : ICustomerWalletService
{
    private readonly ICustomerService _customerService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IRepository<CustomerWallet> _customerWalletRepository;
    private readonly IRepository<CustomerWalletTransaction> _customerWalletTransactionRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IStoreMappingService _storeMappingService;
    private readonly ISettingService _settingService;
    private readonly IStoreService _storeService;
    private readonly IWorkContext _workContext;
    private readonly IOrderService _orderService;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILogger _logger;
    private readonly OrderSettings _orderSettings;

    public CustomerWalletService(ICustomerService customerService,
        IPaymentPluginManager paymentPluginManager,
        IRepository<CustomerWallet> customerWalletRepository,
        IRepository<CustomerWalletTransaction> customerWalletTransactionRepository,
        IEventPublisher eventPublisher,
        IStoreMappingService storeMappingService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWorkContext workContext,
        ICacheKeyService cacheKeyService,
        IGenericAttributeService genericAttributeService,
        IOrderService orderService,
        ILogger logger,
        OrderSettings orderSettings)
    {
        _customerService = customerService;
        _paymentPluginManager = paymentPluginManager;
        _customerWalletRepository = customerWalletRepository;
        _customerWalletTransactionRepository = customerWalletTransactionRepository;
        _eventPublisher = eventPublisher;
        _storeMappingService = storeMappingService;
        _settingService = settingService;
        _storeService = storeService;
        _workContext = workContext;
        _cacheKeyService = cacheKeyService;
        _genericAttributeService = genericAttributeService;
        _orderService = orderService;
        _logger = logger;
        _orderSettings = orderSettings;
    }

    /// <summary>
    /// Validate the last modified date has not changed
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="storeId">Store id</param>
    /// <returns>True if wallet has not been modified; Otherwise, False</returns>
    public virtual async Task<bool> HasCustomerWalletBeenModifiedAsync(int customerId,
        ShoppingCartType shoppingCartType = ShoppingCartType.ShoppingCart,
        int storeId = 0)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return false;

        var shoppingCartWalletLastModifiedKey =_cacheKeyService.PrepareKeyForDefaultCache(
            NopOrderDefaults.ShoppingCartWalletLastModifiedCacheKey,
            customer,
            storeId);

        var walletLastModified =await _genericAttributeService.GetAttributeAsync(customer,
            shoppingCartWalletLastModifiedKey.Key,
            storeId,
            default(long));

        var isModified = false;

        var customerWallet = GetAllCustomerWallets(customerId, storeId: storeId).FirstOrDefault();
        if (customerWallet != null && walletLastModified != 0)
            isModified = walletLastModified != customerWallet.ModifiedDateUtc.Ticks;

        if (isModified)
           await _genericAttributeService.DeleteAttributeAsync(customer, shoppingCartWalletLastModifiedKey.Key, storeId);

        return isModified;
    }

    /// <summary>
    /// Checks that the wallet exists
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="storeId">Store id</param>
    /// <returns>True if wallet exists; Otherwise, False</returns>
    public virtual async Task<bool> HasWalletAsync(int customerId, int storeId = 0)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return false;

        var customerWallet = GetAllCustomerWallets(customerId, storeId: storeId).FirstOrDefault();
        return customerWallet != null;
    }

    /// <summary>
    /// Ensures the amount deducted from the wallet is valid for the order.
    /// </summary>
    /// <param name="customerId">Customer id</param>
    /// <param name="shoppingCartType">Shopping cart type</param>
    /// <param name="storeId">Store id</param>
    public virtual async Task<decimal> EnsureWalletDeductionAmountIsValid(int customerId, int storeId = 0)
    {
        var customer =await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return 0;

        var shoppingCartWalletDeductionKey = _cacheKeyService.PrepareKeyForDefaultCache(
            NopOrderDefaults.ShoppingCartWalletDeductionCacheKey,
            customer,
            storeId);

        var amountToDeduct =await _genericAttributeService.GetAttributeAsync(customer,
            shoppingCartWalletDeductionKey.Key,
            storeId,
            default(decimal));

        var customerWallet = await EnsureCustomerWalletAsync(customerId, storeId);
        var currentAmountCashback = GetAmountExcludingCashback(customerWallet, storeId);


        var hasFundsToUse = currentAmountCashback >= amountToDeduct;
        // add logging
        if (!_orderSettings.OrderLoggingDisabled)
        {
            _logger.Information($"EnsureWalletDeductionAmountIsValid method: has funds to use{hasFundsToUse} customerId {customerId} storeId {storeId} ");
        }
        return hasFundsToUse ? amountToDeduct : 0;
    }
    public virtual async Task<bool> IsRefundCapturedAsync(int customerId, int storeId, int? refundType, decimal? transactionTrackingNumber = null)
    {
        var customerWallet =  await EnsureCustomerWalletAsync(customerId, storeId);
        var existingTransactions = GetAllCustomerWalletTransactions(customerWallet.Id,
        transactionTrackingNumber: transactionTrackingNumber,
        verifiedToArch: true,
        storeId: storeId);
        var alreadyCaptured = existingTransactions.Any(i => i.ArchRefundType == refundType);
        return alreadyCaptured;
    }

    public virtual async Task<CustomerWallet> EnsureCustomerWalletAsync(int customerId, int storeId)
    {
        var customerWallet = GetAllCustomerWallets(customerId, storeId: storeId).FirstOrDefault();
        if (customerWallet == null)
        {
            var wallet = new CustomerWallet
            {
                CustomerId = customerId,
                Amount = 0,
                LimitedToStores = storeId > 0
            };
            InsertCustomerWallet(wallet);

            if (wallet.LimitedToStores)
            {
                var existingStoreMappings =await _storeMappingService.(wallet);
                if (existingStoreMappings.Count(sm => sm.StoreId == storeId) == 0)
                   await _storeMappingService.InsertCustomerWalletStoreMapping(wallet, storeId);
            }

            customerWallet = GetAllCustomerWallets(customerId, storeId: storeId).FirstOrDefault();
        }


        if (!_orderSettings.OrderLoggingDisabled)
        {
            _logger.Information($"EnsureCustomerWallet method: customer wallet amount {customerWallet.Amount}  customerId {customerId} storeId {storeId} ");
        }

        return customerWallet;
    }

    public virtual async Task ArchRefundCustomerWalletAsync(int customerId,
        decimal amount,
        string description,
        WalletTransactionType transactionType,
        int storeId,
        int? refundType,
        Order order,
        decimal? transactionTrackingNumber = null,
        CashbackStatus? cashbackStatus = null)
    {
        try
        {
            var customerWallet =await EnsureCustomerWalletAsync(customerId, storeId);

            // if a full refund is request, then void any existing partial cashback
            if (refundType.HasValue && refundType.Value == 1)
            {
                var existingParitalCashbackTransactions = GetAllCustomerWalletTransactions(customerWallet.Id,
                        transactionTrackingNumber: transactionTrackingNumber, verifiedToArch: true, storeId: storeId)
                    .Where(i => i.ArchRefundType == 0);

                foreach (var toVoid in existingParitalCashbackTransactions)
                {
                    toVoid.CashbackStatus = CashbackStatus.Voided;
                    UpdateCustomerWalletTransaction(toVoid);
                }
            }

            var transaction = new CustomerWalletTransaction
            {
                Amount = amount,
                CustomerWalletId = customerWallet.Id,
                Description = $"[{transactionType}] {description}",
                WalletTransactionTypeId = (int)transactionType,
                TransactionTrackingNumber = transactionTrackingNumber,
                LimitedToStores = storeId > 0,
                CashbackStatusId = (int)cashbackStatus,
                OrderId = order.Id,
                StoreId = storeId,
            };
            InsertCustomerWalletTransaction(transaction);

            if (transaction.LimitedToStores)
            {
                var existingStoreMappings = _storeMappingService.GetCustomerWalletStoreMappings(transaction);
                if (existingStoreMappings.Count(sm => sm.StoreId == storeId) == 0)
                    _storeMappingService.InsertCustomerWalletStoreMapping(transaction, storeId);
            }

            if (amount > 0)
            {
                switch (transactionType)
                {
                    case WalletTransactionType.Credit:
                        customerWallet.Amount += amount;
                        break;
                    case WalletTransactionType.Debit:
                        customerWallet.Amount -= amount;
                        break;
                    case WalletTransactionType.Info:
                    default:
                        break;
                }

                if (!_orderSettings.OrderLoggingDisabled)
                {
                    _logger.Information($"ArchRefundCustomerWallet method: customer wallet amount {customerWallet.Amount}  customerId {customerId} storeId {storeId} ");
                }
                UpdateCustomerWallet(customerWallet);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public bool VoidCashback(int customerWalletTransactionId,
        long LastModifiedTicks,
        out int? customerWalletId)
    {
        var success = false;

        var customerWalletTransaction = GetCustomerWalletTransactionById(customerWalletTransactionId);
        customerWalletId = customerWalletTransaction?.CustomerWalletId;

        if (customerWalletTransaction == null ||
            customerWalletTransaction.CashbackStatus != CashbackStatus.Pending ||
            customerWalletTransaction.ModifiedDateUtc.Ticks != LastModifiedTicks)
            return success;

        var customerWallet = GetCustomerWalletById(customerWalletTransaction.CustomerWalletId);
        if (customerWallet == null)
            return success;

        // update existing transaction cashback status
        customerWalletTransaction.CashbackStatusId = (int)CashbackStatus.Voided;
        customerWalletTransaction.Description += $" / Cashback voided by customer {_workContext.CurrentCustomer.Id}";
        UpdateCustomerWalletTransaction(customerWalletTransaction);

        // create a new transaction to record the debiting of the wallet
        var transaction = new CustomerWalletTransaction
        {
            Amount = customerWalletTransaction.Amount,
            CustomerWalletId = customerWallet.Id,
            Description = $"[{WalletTransactionType.Info}] Cashback voided by customer {_workContext.CurrentCustomer.Id} for transaction {customerWalletTransaction.Id}",
            WalletTransactionTypeId = (int)WalletTransactionType.Info,
            TransactionTrackingNumber = customerWalletTransaction.TransactionTrackingNumber,
            LimitedToStores = customerWalletTransaction.LimitedToStores,
            CashbackStatusId = (int)CashbackStatus.NotApplicable,
            OrderId = customerWalletTransaction.OrderId,
            StoreId = customerWalletTransaction.StoreId,
        };
        InsertCustomerWalletTransaction(transaction);

        if (transaction.LimitedToStores)
        {
            var existingStoreMappings = _storeMappingService.GetCustomerWalletStoreMappings(customerWalletTransaction);
            foreach (var storeMapping in existingStoreMappings)
            {
                _storeMappingService.InsertCustomerWalletStoreMapping(transaction, storeMapping.StoreId);
            }
        }

        return success;
    }

    public virtual async Task<bool> ClaimCashback(int customerWalletTransactionId,
        long LastModifiedTicks,
        out int? customerWalletId)
    {
        var success = false;

        var customerWalletTransaction = GetCustomerWalletTransactionById(customerWalletTransactionId);
        customerWalletId = customerWalletTransaction?.CustomerWalletId;

        if (customerWalletTransaction == null ||
            customerWalletTransaction.CashbackStatus != CashbackStatus.Pending ||
            customerWalletTransaction.ModifiedDateUtc.Ticks != LastModifiedTicks)
            return success;

        var customerWallet = GetCustomerWalletById(customerWalletTransaction.CustomerWalletId);
        if (customerWallet == null)
            return success;

        if (!customerWalletTransaction.TransactionTrackingNumber.HasValue)
            return success;

        var order =await _orderService.GetOrderByTransactionTrackingNumber(customerWalletTransaction.TransactionTrackingNumber.Value, storeId: customerWalletTransaction.StoreId.Value);
        if (order == null)
            return success;

        var customer = await _customerService.GetCustomerByIdAsync(customerWallet.CustomerId);


        var paymentMethod =await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, customer, (int)customerWalletTransaction.StoreId);
        var canClaimCashback = paymentMethod != null && paymentMethod.SupportPartiallyRefund;
        if (canClaimCashback)
        {
            var refundResult = paymentMethod.Refund(new RefundPaymentRequest
            {
                AmountToRefund = customerWalletTransaction.Amount,
                IsPartialRefund = true,
                Order = order
            });

            success = refundResult.Success;
            if (!success)
            {
                var errors = string.Join(Environment.NewLine, refundResult.Errors);
                _logger.Error(errors, new NopException($"Cashback claim unsuccessful for order {order.Id} and customerWalletTransaction {customerWalletTransaction.Id} for amount {customerWalletTransaction.Amount}"));

               await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Cashback claim unsuccessful for order {order.Id} and customerWalletTransaction {customerWalletTransaction.Id} for amount {customerWalletTransaction.Amount}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

               await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Cashback claim unsuccessful: {errors}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                return success;
            }
            else
            {
               await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Cashback claim successful for order {order.Id} and customerWalletTransaction {customerWalletTransaction.Id} for amount {customerWalletTransaction.Amount}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }

            customerWalletTransaction.CashbackStatusId = (int)CashbackStatus.Claimed;
            customerWalletTransaction.Description += $" / Cashback claimed by customer {_workContext.CurrentCustomer.Id}";
            UpdateCustomerWalletTransaction(customerWalletTransaction);

            // create a new transaction to record the debiting of the wallet
            var transaction = new CustomerWalletTransaction
            {
                Amount = customerWalletTransaction.Amount,
                CustomerWalletId = customerWallet.Id,
                Description = $"[{WalletTransactionType.Debit}] Cashback claimed by customer {_workContext.CurrentCustomer.Id}",
                WalletTransactionTypeId = (int)WalletTransactionType.Debit,
                TransactionTrackingNumber = customerWalletTransaction.TransactionTrackingNumber,
                LimitedToStores = customerWalletTransaction.LimitedToStores,
                CashbackStatusId = (int)CashbackStatus.NotApplicable,
                OrderId = order.Id,
                StoreId = order.StoreId
            };
            InsertCustomerWalletTransaction(transaction);

            if (transaction.LimitedToStores)
            {
                var existingStoreMappings = _storeMappingService.GetCustomerWalletStoreMappings(customerWalletTransaction);
                foreach (var storeMapping in existingStoreMappings)
                {
                    _storeMappingService.InsertCustomerWalletStoreMapping(transaction, storeMapping.StoreId);
                }
            }

            customerWallet.Amount = customerWallet.Amount > 0 ? customerWallet.Amount -= customerWalletTransaction.Amount : 0;
            UpdateCustomerWallet(customerWallet);

            return success;
        }
        else
        {
            // resolve the money into their wallet if we enter this situation where the payment gateway cannot do partial refunds
            return VoidCashback(customerWalletTransactionId, LastModifiedTicks, out customerWalletId);
        }
    }

    public virtual async Task<decimal> DeductCustomerWalletForOrder(Order order)
    {
        var shoppingCartWalletDeductionKey = _cacheKeyService.PrepareKeyForShortTermCache(NopOrderDefaults.ShoppingCartWalletDeductionCacheKey,
            _workContext.CurrentCustomer, order.StoreId);

        var customerWallet = EnsureCustomerWallet(order.CustomerId, order.StoreId);
        var customer = _customerService.GetCustomerById(order.CustomerId);
        var walleteBallanceBefore = customerWallet.Amount.ToString("#0.00");
        var amountToDeduct = _genericAttributeService.GetAttribute(customer,
            shoppingCartWalletDeductionKey.Key,
            order.StoreId,
            default(decimal));

        if (order.CustomerWalletDeduction.HasValue && amountToDeduct == 0)
            amountToDeduct = order.CustomerWalletDeduction.Value;

        if (amountToDeduct > 0)
        {
            var transaction = new CustomerWalletTransaction
            {
                Amount = amountToDeduct,
                CustomerWalletId = customerWallet.Id,
                Description = $"[{WalletTransactionType.Debit}] For order {order.Id}",
                WalletTransactionTypeId = (int)WalletTransactionType.Debit,
                TransactionTrackingNumber = order.TransactionTrackingNumber,
                LimitedToStores = customerWallet.LimitedToStores,
                CashbackStatusId = (int)CashbackStatus.NotApplicable,
                OrderId = order.Id,
                StoreId = order.StoreId,
            };
            InsertCustomerWalletTransaction(transaction);

            if (transaction.LimitedToStores)
            {
                var existingStoreMappings = _storeMappingService.GetCustomerWalletStoreMappings(customerWallet);
                foreach (var storeMapping in existingStoreMappings)
                {
                    _storeMappingService.InsertCustomerWalletStoreMapping(transaction, storeMapping.StoreId);
                }
            }

            customerWallet.Amount = customerWallet.Amount > 0 ? customerWallet.Amount -= amountToDeduct : 0;
            UpdateCustomerWallet(customerWallet);
        }

        var shoppingCartWalletLastModifiedKey = _cacheKeyService.PrepareKeyForShortTermCache(NopOrderDefaults.ShoppingCartWalletLastModifiedCacheKey,
            customer, order.StoreId);

        _genericAttributeService.DeleteAttributes(customer, shoppingCartWalletLastModifiedKey.Key, order.StoreId);
        _genericAttributeService.DeleteAttributes(customer, shoppingCartWalletDeductionKey.Key, order.StoreId);

        if (!_orderSettings.OrderLoggingDisabled)
        {
            _logger.Information($"DeductCustomerWalletForOrder method: Current Wallet Ballance({walleteBallanceBefore}), amount to deduct {amountToDeduct} order id {order.Id}  TTN {order.TransactionTrackingNumber} storeId {order.StoreId} ");
        }

        return amountToDeduct;
    }

    public virtual void DeleteCustomerWallet(CustomerWallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        var existingStoreMappings = _storeMappingService.GetCustomerWalletStoreMappings(wallet);
        var allStores = _storeService.GetAllStores();
        foreach (var store in allStores)
        {
            var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
            if (storeMappingToDelete != null)
                _storeMappingService.DeleteCustomerWalletStoreMapping(storeMappingToDelete);
        }

        _customerWalletRepository.Delete(wallet);

        //event notification
        _eventPublisher.EntityDeleted(wallet);
    }

    public virtual CustomerWallet GetCustomerWalletById(int customerWalletId)
    {
        if (customerWalletId == 0)
            return null;

        return _customerWalletRepository.ToCachedGetById(customerWalletId);
    }

    public virtual IPagedList<CustomerWallet> SearchCustomerWallets(int? customerId,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
        var query = _customerWalletRepository.Table;

        if (customerId.HasValue)
            query = query.Where(w => w.CustomerId == customerId.Value);

        query = _storeMappingService.FilterStoresForCustomerWallet(query, storeId);

        query = query.OrderBy(t => t.CreatedDateUtc);

        //filter by dates
        if (fromDateUtc.HasValue)
            query = query.Where(discount => discount.CreatedDateUtc >= fromDateUtc.Value);
        if (toDateUtc.HasValue)
            query = query.Where(discount => discount.CreatedDateUtc <= toDateUtc.Value);

        //database layer paging
        return new PagedList<CustomerWallet>(query, pageIndex, pageSize, getOnlyTotalCount);
    }

    public virtual IPagedList<CustomerWalletTransaction> SearchCustomerWalletTransactions(int customerWalletId,
        List<int> cashbackStatusIds, List<int> transactionTypeIds,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
    {
        var query = _customerWalletTransactionRepository.Table;

        query = query.Where(w => w.CustomerWalletId == customerWalletId);

        query = _storeMappingService.FilterStoresForCustomerWallet(query, storeId);

        if (cashbackStatusIds != null && cashbackStatusIds.Any())
            query = query.Where(t => cashbackStatusIds.Contains(t.CashbackStatusId));

        if (transactionTypeIds != null && transactionTypeIds.Any())
            query = query.Where(t => transactionTypeIds.Contains(t.WalletTransactionTypeId));

        query = query.OrderBy(t => t.CreatedDateUtc);

        //filter by dates
        if (fromDateUtc.HasValue)
            query = query.Where(discount => discount.CreatedDateUtc >= fromDateUtc.Value);
        if (toDateUtc.HasValue)
            query = query.Where(discount => discount.CreatedDateUtc <= toDateUtc.Value);

        //database layer paging
        return new PagedList<CustomerWalletTransaction>(query, pageIndex, pageSize, getOnlyTotalCount);
    }

    public virtual IList<CustomerWallet> GetAllCustomerWallets(int customerId,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0)
    {
        if (customerId == 0)
            return new List<CustomerWallet>();

        var query = _customerWalletRepository.Table;

        query = query.Where(w => w.CustomerId == customerId);

        if (!_orderSettings.IsWalletSharedAcrossStores)
            query = _storeMappingService.FilterStoresForCustomerWallet(query, storeId);

        query = query.OrderBy(t => t.CreatedDateUtc);

        //filter by dates
        if (fromDateUtc.HasValue)
            query = query.Where(discount => discount.CreatedDateUtc >= fromDateUtc.Value);
        if (toDateUtc.HasValue)
            query = query.Where(discount => discount.CreatedDateUtc <= toDateUtc.Value);

        return query.ToList();
    }
    private IQueryable<CustomerWallet> GetCustomerWalletTransactionByStoreId(IQueryable<CustomerWallet> query, int storeId = 0)
    {
        var walletTransactionQuery = _customerWalletTransactionRepository.Table;

        if (storeId > 0)
            walletTransactionQuery = walletTransactionQuery.Where(w => w.StoreId == storeId);

        var joinedQuery = from wallet in query
                          join transaction in walletTransactionQuery
                          on wallet.Id equals transaction.CustomerWalletId
                          select wallet;

        return joinedQuery.Distinct();
    }

    public virtual decimal GetAmountExcludingCashback(CustomerWallet customerWallet, int storeId)
    {
        var cashbackTransactions = GetAllCustomerWalletTransactions(customerWallet.Id,
            WalletTransactionType.Credit,
            cashbackStatus: CashbackStatus.Pending,
            verifiedToArch: true,
            storeId: storeId);
        var cashbackAmount = cashbackTransactions.Sum(i => i.Amount);

        var amount = customerWallet.Amount - cashbackAmount;

        return amount > 0 ? amount : 0.00m;
    }

    public virtual void InsertCustomerWallet(CustomerWallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        if (wallet.CustomerId <= 0)
            throw new ArgumentException("CustomerId must be a valid customer identifier.", nameof(wallet.CustomerId));

        wallet.CreatedDateUtc = DateTime.UtcNow;
        wallet.ModifiedDateUtc = DateTime.UtcNow;

        _customerWalletRepository.Insert(wallet);

        //event notification
        _eventPublisher.EntityInserted(wallet);
    }

    public virtual void UpdateCustomerWallet(CustomerWallet wallet)
    {
        if (wallet == null)
            throw new ArgumentNullException(nameof(wallet));

        if (wallet.Amount < 0)
            throw new ArgumentException("Wallet Amount can't be less than 0", nameof(wallet.Amount));

        wallet.ModifiedDateUtc = DateTime.UtcNow;
        _customerWalletRepository.Update(wallet);

        //event notification
        _eventPublisher.EntityUpdated(wallet);
    }

    public virtual void DeleteCustomerWalletTransaction(CustomerWalletTransaction walletTransaction)
    {
        if (walletTransaction == null)
            throw new ArgumentNullException(nameof(walletTransaction));

        var existingStoreMappings = _storeMappingService.GetCustomerWalletStoreMappings(walletTransaction);
        var allStores = _storeService.GetAllStores();
        foreach (var store in allStores)
        {
            var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
            if (storeMappingToDelete != null)
                _storeMappingService.DeleteCustomerWalletStoreMapping(storeMappingToDelete);

        }
        _customerWalletTransactionRepository.Delete(walletTransaction);

        //event notification
        _eventPublisher.EntityDeleted(walletTransaction);
    }

    public virtual CustomerWalletTransaction GetCustomerWalletTransactionById(int customerWalletTransactionId)
    {
        if (customerWalletTransactionId == 0)
            return null;

        return _customerWalletTransactionRepository.ToCachedGetById(customerWalletTransactionId);
    }

    public virtual IList<CustomerWalletTransaction> GetAllCustomerWalletTransactions(int? customerWalletId = null,
        WalletTransactionType? transactionType = null,
        decimal? transactionTrackingNumber = null, bool? verifiedToArch = null,
        DateTime? fromDateUtc = null, DateTime? toDateUtc = null, int storeId = 0,
        CashbackStatus? cashbackStatus = null)
    {
        var query = _customerWalletTransactionRepository.Table;

        if (customerWalletId.HasValue && customerWalletId > 0)
            query = query.Where(w => w.CustomerWalletId == customerWalletId);

        if (storeId > 0)
            query = query.Where(t => t.StoreId == storeId);

        if (verifiedToArch.HasValue)
            query = query.Where(t => t.ArchRefundType != null);

        if (transactionTrackingNumber.HasValue)
            query = query.Where(t => t.TransactionTrackingNumber == transactionTrackingNumber.Value);

        query = query.OrderBy(t => t.CreatedDateUtc);

        if (transactionType.HasValue)
            query = query.Where(t => t.WalletTransactionTypeId == (int)transactionType.Value);

        if (cashbackStatus.HasValue)
            query = query.Where(t => t.CashbackStatusId == (int)cashbackStatus.Value);

        //filter by dates
        if (fromDateUtc.HasValue)
            query = query.Where(t => t.CreatedDateUtc >= fromDateUtc.Value);
        if (toDateUtc.HasValue)
            query = query.Where(t => t.CreatedDateUtc <= toDateUtc.Value);

        return query.OrderByDescending(i => i.CreatedDateUtc).ToList();
    }

    public virtual void InsertCustomerWalletTransaction(CustomerWalletTransaction walletTransaction)
    {
        if (walletTransaction == null)
            throw new ArgumentNullException(nameof(walletTransaction));

        if (walletTransaction.CustomerWalletId <= 0)
            throw new ArgumentException("CustomerWalletId must be a valid customer wallet identifier.", nameof(walletTransaction.CustomerWalletId));

        walletTransaction.CreatedDateUtc = DateTime.UtcNow;
        walletTransaction.ModifiedDateUtc = DateTime.UtcNow;
        _customerWalletTransactionRepository.Insert(walletTransaction);

        //event notification
        _eventPublisher.EntityInserted(walletTransaction);
    }

    public virtual void UpdateCustomerWalletTransaction(CustomerWalletTransaction walletTransaction)
    {
        if (walletTransaction == null)
            throw new ArgumentNullException(nameof(walletTransaction));

        if (walletTransaction.CustomerWalletId <= 0)
            throw new ArgumentException("CustomerWalletId must be a valid customer wallet identifier.", nameof(walletTransaction.CustomerWalletId));

        walletTransaction.ModifiedDateUtc = DateTime.UtcNow;
        _customerWalletTransactionRepository.Update(walletTransaction);
        // add logging
        //event notification
        _eventPublisher.EntityUpdated(walletTransaction);
    }
}
