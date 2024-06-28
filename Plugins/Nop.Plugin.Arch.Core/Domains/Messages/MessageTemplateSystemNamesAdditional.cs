using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Messages;
public static class MessageTemplateSystemNamesAdditional
{

    /// <summary>
    /// Represents system name of notification about new cashback for customer
    /// </summary>
    public const string CUSTOMER_CASHBACK_NOTIFICATION = "Customer.CashbackNotification";

    public const string APP_DOWNLOAD_MESSAGE = "Driver.AppDownload";

    public const string CUSTOMER_ONE_TIME_PIN_MESSAGE = "SMS.Customer.OneTimePin";
    public const string CUSTOMER_ORDER_READY_FOR_COLLECTION_MESSAGE = "SMS.Customer.OrderReadyForCollection";
    public const string CUSTOMER_ORDER_OUT_FOR_DELIVERY_MESSAGE = "SMS.Customer.OrderOutForDelivery";
    public const string ABANDONED_CART_REMINDER_NOTIFICATION = "ShoppingCart.AbandonedCartReminderNotification";
    public const string WISHLIST_ITEM_SALE_NOTIFICATION = "ShoppingCart.WishlistItemSaleNotification";
    /// <summary>
    /// Represents system name of notification store owner about placed order
    /// </summary>
    public const string ORDER_PLACED_STORE_MESSAGE_NOTIFICATION = "OrderPlaced.StoreMessageNotification";
    /// <summary>
    /// Represents system name of notification store owner about cancelled order
    /// </summary>
    public const string ORDER_CANCELLED_STORE_OWNER_NOTIFICATION = "OrderCancelled.StoreOwnerNotification";
}
