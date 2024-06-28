using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Arch.Core.Services.Security;
/// <summary>
/// Standard permission provider
/// </summary>
public partial class ArchStandardPermissionProvider : StandardPermissionProvider
{
    //admin area permissions

    public static readonly PermissionRecord ManagePromotionGroups = new PermissionRecord { Name = "Admin area. Manage Promotion Groups", SystemName = "ManagePromotionGroups", Category = "Promo" };
    public static readonly PermissionRecord ManageRecommendedLists = new PermissionRecord { Name = "Admin area. Manage Recommended Lists", SystemName = "ManageRecommendedLists", Category = "Catalog" };


    /// <summary>
    /// Get permissions
    /// </summary>
    /// <returns>Permissions</returns>
    public override IEnumerable<PermissionRecord> GetPermissions()
    {
        return new[]
        {
                AccessAdminPanel,
                AllowCustomerImpersonation,
                ManageProducts,
                ManageCategories,
                ManageManufacturers,
                ManagePromotionGroups,
                ManageRecommendedLists,
                ManageProductReviews,
                ManageProductTags,
                ManageAttributes,
                ManageCustomers,
                ManageVendors,
                ManageCurrentCarts,
                ManageOrders,
                ManageRecurringPayments,
                ManageGiftCards,
                ManageReturnRequests,
                OrderCountryReport,
                ManageAffiliates,
                ManageCampaigns,
                ManageDiscounts,
                ManageNewsletterSubscribers,
                ManagePolls,
                ManageNews,
                ManageBlog,
                ManageWidgets,
                ManageTopics,
                ManageForums,
                ManageMessageTemplates,
                ManageCountries,
                ManageLanguages,
                ManageSettings,
                ManagePaymentMethods,
                ManageExternalAuthenticationMethods,
                ManageTaxSettings,
                ManageShippingSettings,
                ManageCurrencies,
                ManageActivityLog,
                ManageAcl,
                ManageEmailAccounts,
                ManageStores,
                ManagePlugins,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
                HtmlEditorManagePictures,
                ManageScheduleTasks,
                DisplayPrices,
                EnableShoppingCart,
                EnableWishlist,
                PublicStoreAllowNavigation,
                AccessClosedStore
            };
    }

    /// <summary>
    /// Get default permissions
    /// </summary>
    /// <returns>Permissions</returns>
    public override HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
    {
        return new HashSet<(string, PermissionRecord[])>
            {
                (
                    NopCustomerDefaults.AdministratorsRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        AllowCustomerImpersonation,
                        ManageProducts,
                        ManageCategories,
                        ManageManufacturers,
                        ManagePromotionGroups,
                        ManageRecommendedLists,
                        ManageProductReviews,
                        ManageProductTags,
                        ManageAttributes,
                        ManageCustomers,
                        ManageVendors,
                        ManageCurrentCarts,
                        ManageOrders,
                        ManageRecurringPayments,
                        ManageGiftCards,
                        ManageReturnRequests,
                        OrderCountryReport,
                        ManageAffiliates,
                        ManageCampaigns,
                        ManageDiscounts,
                        ManageNewsletterSubscribers,
                        ManagePolls,
                        ManageNews,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageForums,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManagePaymentMethods,
                        ManageExternalAuthenticationMethods,
                        ManageTaxSettings,
                        ManageShippingSettings,
                        ManageCurrencies,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageStores,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation,
                        AccessClosedStore
                    }
                ),
                (
                    NopCustomerDefaults.ForumModeratorsRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    NopCustomerDefaults.GuestsRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    NopCustomerDefaults.RegisteredRoleName,
                    new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                ),
                (
                    NopCustomerDefaults.VendorsRoleName,
                    new[]
                    {
                        AccessAdminPanel,
                        ManageProducts,
                        ManageProductReviews,
                        ManageOrders
                    }
                )
            };
    }
}
