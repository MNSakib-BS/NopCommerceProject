using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders;
using Nop.Plugin.Arch.Core.Domains.Affiliates;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.Discounts;
using Nop.Plugin.Arch.Core.Domains.Forums;
using Nop.Plugin.Arch.Core.Domains.Gdpr;
using Nop.Plugin.Arch.Core.Domains.Logging;
using Nop.Plugin.Arch.Core.Domains.Messages;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Payments;
using Nop.Plugin.Arch.Core.Domains.Promotions;
using Nop.Plugin.Arch.Core.Domains.Shipping;
using Nop.Plugin.Arch.Core.Domains.Stores;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Domains.Topics;
using Nop.Plugin.Arch.Core.Domains.Vendors;
using Nop.Plugin.Arch.Core.Domains.WishlistItemSaleNotifiers;

namespace Nop.Plugin.Arch.Core.Data.Migrations;

[NopMigration("2024/05/09 12:00:00", "Arch Core base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<AffiliateAdditional>();
        Create.TableFor<AbandonedCartReminder>();
        Create.TableFor<AbandonedCartReminderQueue>();
        Create.TableFor<ArchProductDetail>();
        Create.TableFor<ArchStoreProductInfo>();
        Create.TableFor<CategoryAdditional>();
        Create.TableFor<CategoryTemplateAdditional>();
        Create.TableFor<ManufacturerTemplateAdditional>();
        Create.TableFor<PredefinedProductAttributeValueAdditional>();
        Create.TableFor<ProductAdditional>();
        Create.TableFor<ProductAttributeAdditional>();
        Create.TableFor<ProductAttributeMappingAdditional>();
        Create.TableFor<ProductAttributeValueAdditional>();
        Create.TableFor<ProductCategoryAdditional>();
        Create.TableFor<ProductManufacturerAdditional>();
        Create.TableFor<ProductReviewAdditional>();
        Create.TableFor<ProductSpecificationAttributeAdditional>();
        Create.TableFor<ProductTagAdditional>();
        Create.TableFor<ProductTemplateAdditional>();
        Create.TableFor<PromotedProductCategory>();
        Create.TableFor<RecommendedList>();
        Create.TableFor<SpecificationAttributeAdditional>();
        Create.TableFor<TierPriceAdditional>();
        Create.TableFor<CustomerAdditional>();
        Create.TableFor<CustomerAttributeAdditional>();
        Create.TableFor<CustomerPicture>();
        Create.TableFor<DiscountAdditional>();
        Create.TableFor<ForumAdditional>();
        Create.TableFor<ForumGroupAdditional>();
        Create.TableFor<ForumPostAdditional>();
        Create.TableFor<ForumTopicAdditional>();
        Create.TableFor<GdprConsentAdditional>();
        Create.TableFor<GdprLogAdditional>();
        Create.TableFor<ActivityLogAdditional>();
        Create.TableFor<EmailAccountAdditional>();
        Create.TableFor<QueuedEmailAdditional>();
        Create.TableFor<ArchInvoiceItem>();
        Create.TableFor<ArchQuotationItem>();
        Create.TableFor<AvailableDeliveryDateTimeRange>();
        Create.TableFor<AvailableDeliveryTimeRange>();
        Create.TableFor<ExceptionDate>();
        Create.TableFor<OrderAdditional>();
        Create.TableFor<OrderNoteAdditional>();
        Create.TableFor<OrderShippingMethodCapacityMapping>();
        Create.TableFor<ProductRecommendedList>();
        Create.TableFor<RL_DS_AvailableDeliveryDateTimeRange>();
        Create.TableFor<RL_DS_AvailableDeliveryTimeRange>();
        Create.TableFor<RL_DS_OrderShippingMethodCapacityMapping>();
        Create.TableFor<RL_DS_ShippingMethodCapacity>();
        Create.TableFor<ShippingMethodCapacity>();
        Create.TableFor<ShoppingCartItemAdditional>();
        Create.TableFor<WishListGroup>();
        Create.TableFor<CustomerWallet>();
        Create.TableFor<CustomerWalletTransaction>();
        Create.TableFor<ProductPromotionGroup>();
        Create.TableFor<PromotionGroup>();
        Create.TableFor<PromotionGroupTemplate>();
        Create.TableFor<PromotionGroupWidgetZones>();
        Create.TableFor<ShipmentAdditional>();
        Create.TableFor<CustomerWalletStoreMapping>();
        Create.TableFor<StoreAdditional>();
        Create.TableFor<StoreType>();
        Create.TableFor<StoreTypeMapping>();
        Create.TableFor<TopicAdditional>();
        Create.TableFor<VendorAdditional>();
        Create.TableFor<WishlistItemSaleNotifier>();
        Create.TableFor<WishlistItemSaleNotifierQueue>();
    }

    #endregion
}
