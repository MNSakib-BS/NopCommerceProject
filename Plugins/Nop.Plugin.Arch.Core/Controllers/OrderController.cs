using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Arch.Core.Controllers;

[AutoValidateAntiforgeryToken]
public partial class OrderController : BasePublicController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderModelFactory _orderModelFactory;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentService _paymentService;
    protected readonly IPdfService _pdfService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly RewardPointsSettings _rewardPointsSettings;

    #endregion

    #region Ctor

    public OrderController(ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderModelFactory orderModelFactory,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentService paymentService,
        IPdfService pdfService,
        IShipmentService shipmentService,
        IWebHelper webHelper,
        IWorkContext workContext,
        RewardPointsSettings rewardPointsSettings)
    {
        _customerService = customerService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderModelFactory = orderModelFactory;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _paymentService = paymentService;
        _pdfService = pdfService;
        _shipmentService = shipmentService;
        _webHelper = webHelper;
        _workContext = workContext;
        _rewardPointsSettings = rewardPointsSettings;
    }

    #endregion

    #region Methods
    [HttpsRequirement]
    public virtual IActionResult GetCustomerOrders(int orderStatusId, int paymentStatusId, int shippingStatusId, int pageIndex, int tabIndex)
    {
        var filteringModel = new CustomerOrderPagingFilteringModel()
        {
            PageNumber = pageIndex + 1,
            TabIndex = tabIndex
        };

        var model = _orderModelFactory.PrepareCustomerOrderListModel(filteringModel);

        var orderStatus = orderStatusId == 0 ? null : (OrderStatus?)orderStatusId;
        var paymentStatus = paymentStatusId == 0 ? null : (PaymentStatus?)paymentStatusId;
        var shippingStatus = shippingStatusId == 0 ? null : (ShippingStatus?)shippingStatusId;

        var filteredModel = _orderModelFactory.PrepareCustomerOrderListModel(model, orderStatus, paymentStatus, shippingStatus);

        var themeName = _themeContext.WorkingThemeName;

        return themeName == "DefaultClean"
            ? View("~/Views/Order/_OrderHistoryItem.cshtml", filteredModel)
            : View($"Themes/{themeName}/Views/Order/_OrderHistoryItem.cshtml", filteredModel);
    }

    //My account / Orders
    [HttpsRequirement]
    public virtual IActionResult CustomerOrdersPaged(int pageNumber, int tabIndex)
    {
        if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
            return Challenge();

        var filtering = new CustomerOrderPagingFilteringModel()
        {
            PageNumber = pageNumber,
            TabIndex = tabIndex
        };

        var model = _orderModelFactory.PrepareCustomerOrderListModel(filtering);
        return View(model);
    }

    //My account / Orders
    public virtual async Task<IActionResult> CustomerOrders()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _orderModelFactory.PrepareCustomerOrderListModelAsync();
        return View(model);
    }

    //My account / Orders / Cancel recurring order
    [HttpPost, ActionName("CustomerOrders")]
    [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
    public virtual async Task<IActionResult> CancelRecurringPayment(IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //get recurring payment identifier
        var recurringPaymentId = 0;
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                recurringPaymentId = Convert.ToInt32(formValue["cancelRecurringPayment".Length..]);

        var recurringPayment = await _orderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
        if (recurringPayment == null)
        {
            return RedirectToRoute("CustomerOrders");
        }

        if (await _orderProcessingService.CanCancelRecurringPaymentAsync(customer, recurringPayment))
        {
            var errors = await _orderProcessingService.CancelRecurringPaymentAsync(recurringPayment);

            var model = await _orderModelFactory.PrepareCustomerOrderListModelAsync();
            model.RecurringPaymentErrors = errors;

            return View(model);
        }

        return RedirectToRoute("CustomerOrders");
    }
    public ActionResult Cancel(int orderId)
    {
        if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
            return Challenge();

        var order = _orderService.GetOrderById(orderId);

        if (_orderProcessingService.CanCancelOrder(order))
        {

            if (_archApi.DiscardOrder(order.TransactionTrackingNumber, out var _))
                _orderProcessingService.CancelOrder(order, true, $"ID - {_workContext.CurrentCustomer.Id} Username - {_workContext.CurrentCustomer.Username}");
        }

        return RedirectToRoute("CustomerOrders");
    }

    [HttpPost, ActionName("Search")]
    [AutoValidateAntiforgeryToken]
    public virtual IActionResult Search(IFormCollection form)
    {
        if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
            return Challenge();

        int.TryParse(form["storeId"], out int storeId);
        var searchFilter = form["searchFilter"];

        var model = _orderModelFactory.PrepareCustomerOrderListModel(new CustomerOrderPagingFilteringModel() { StoreId = storeId, SearchFilter = searchFilter });
        return View("CustomerOrders", model);
    }

    //My account / Orders / Retry last recurring order
    [HttpPost, ActionName("CustomerOrders")]
    [FormValueRequired(FormValueRequirement.StartsWith, "retryLastPayment")]
    public virtual async Task<IActionResult> RetryLastRecurringPayment(IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //get recurring payment identifier
        var recurringPaymentId = 0;
        if (!form.Keys.Any(formValue => formValue.StartsWith("retryLastPayment", StringComparison.InvariantCultureIgnoreCase) &&
                                        int.TryParse(formValue[(formValue.IndexOf('_') + 1)..], out recurringPaymentId)))
        {
            return RedirectToRoute("CustomerOrders");
        }

        var recurringPayment = await _orderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
        if (recurringPayment == null)
            return RedirectToRoute("CustomerOrders");

        if (!await _orderProcessingService.CanRetryLastRecurringPaymentAsync(customer, recurringPayment))
            return RedirectToRoute("CustomerOrders");

        var errors = await _orderProcessingService.ProcessNextRecurringPaymentAsync(recurringPayment);
        var model = await _orderModelFactory.PrepareCustomerOrderListModelAsync();
        model.RecurringPaymentErrors = errors.ToList();

        return View(model);
    }

    //My account / Reward points
    public virtual async Task<IActionResult> CustomerRewardPoints(int? pageNumber)
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!_rewardPointsSettings.Enabled)
            return RedirectToRoute("CustomerInfo");

        var model = await _orderModelFactory.PrepareCustomerRewardPointsAsync(pageNumber);
        return View(model);
    }

    //My account / Order details page
    public virtual async Task<IActionResult> Details(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
        return View(model);
    }

    //My account / Order details page / Print
    public virtual async Task<IActionResult> PrintOrderDetails(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
        model.PrintMode = true;

        return View("Details", model);
    }
    //Claim a customer wallet transaction cashback on the order
    public virtual IActionResult ClaimCashback(int orderId,
        int walletTransactionId,
        string lastModifiedTicks)
    {
        var success = _customerWalletService.ClaimCashback(walletTransactionId,
            Convert.ToInt64(lastModifiedTicks),
            out var customerWalletId);

        return RedirectToRoute("OrderDetails", new { orderId = orderId });
    }

    //Void a customer wallet transaction cashback on the order
    public virtual IActionResult VoidCashback(int orderId,
        int walletTransactionId,
        string lastModifiedTicks)
    {
        var success = _customerWalletService.VoidCashback(walletTransactionId,
            Convert.ToInt64(lastModifiedTicks),
            out var customerWalletId);

        return RedirectToRoute("OrderDetails", new { orderId = orderId });
    }

    //Void all customer wallet transaction cashback available
    public virtual IActionResult VoidAllCashback(int customerId)
    {
        var customerWallet = _customerWalletService.GetAllCustomerWallets(customerId, storeId: _storeContext.CurrentStore.Id).FirstOrDefault();
        if (customerWallet != null)
        {
            var customerWalletTransactions = _customerWalletService.GetAllCustomerWalletTransactions(customerWallet.Id,
                WalletTransactionType.Credit,
                cashbackStatus: CashbackStatus.Pending,
                verifiedToArch: true,
                storeId: _storeContext.CurrentStore.Id);

            foreach (var transaction in customerWalletTransactions)
            {
                var success = _customerWalletService.VoidCashback(transaction.Id,
                    transaction.ModifiedDateUtc.Ticks,
                    out var customerWalletId);
            }
        }

        return RedirectToRoute("CustomerOrders");
    }

    //Claim all customer wallet transaction cashback available
    public virtual IActionResult ClaimAllCashback(int customerId)
    {
        var customerWallet = _customerWalletService.GetAllCustomerWallets(customerId, storeId: _storeContext.CurrentStore.Id).FirstOrDefault();
        if (customerWallet != null)
        {
            var customerWalletTransactions = _customerWalletService.GetAllCustomerWalletTransactions(customerWallet.Id,
                WalletTransactionType.Credit,
                cashbackStatus: CashbackStatus.Pending,
                verifiedToArch: true,
                storeId: _storeContext.CurrentStore.Id);

            foreach (var transaction in customerWalletTransactions)
            {
                var success = _customerWalletService.ClaimCashback(transaction.Id,
                    transaction.ModifiedDateUtc.Ticks,
                    out var customerWalletId);
            }
        }

        return RedirectToRoute("CustomerOrders");
    }

    //My account / Order details page / PDF invoice
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetPdfInvoice(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        byte[] bytes;
        await using (var stream = new MemoryStream())
        {
            await _pdfService.PrintOrderToPdfAsync(stream, order, await _workContext.GetWorkingLanguageAsync());
            bytes = stream.ToArray();
        }
        return File(bytes, MimeTypes.ApplicationPdf, string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf");
    }

    //My account / Order details page / re-order
    public virtual async Task<IActionResult> ReOrder(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        var warnings = await _orderProcessingService.ReOrderAsync(order);

        if (warnings.Any())
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("ShoppingCart.ReorderWarning"));

        return RedirectToRoute("ShoppingCart");
    }

    //My account / Order details page / Complete payment
    [HttpPost, ActionName("Details")]

    [FormValueRequired("repost-payment")]
    public virtual async Task<IActionResult> RePostPayment(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        if (!await _paymentService.CanRePostProcessPaymentAsync(order))
            return RedirectToRoute("OrderDetails", new { orderId = orderId });

        var postProcessPaymentRequest = new PostProcessPaymentRequest
        {
            Order = order
        };
        await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

        if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
        {
            //redirection or POST has been done in PostProcessPayment
            return Content("Redirected");
        }

        //if no redirection has been done (to a third-party payment page)
        //theoretically it's not possible
        return RedirectToRoute("OrderDetails", new { orderId = orderId });
    }

    //My account / Order details page / Shipment details page
    public virtual async Task<IActionResult> ShipmentDetails(int shipmentId)
    {
        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
        if (shipment == null)
            return Challenge();

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        var model = await _orderModelFactory.PrepareShipmentDetailsModelAsync(shipment);
        return View(model);
    }

    #endregion
}