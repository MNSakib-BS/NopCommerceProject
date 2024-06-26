using FluentMigrator;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Migrations
{
   
    
    [NopSchemaMigration("2023/09/12 05:02:00:0000011", "DeliveryScheduling base schema,AvailableDeliveryDateTimeRange", MigrationProcessType.Installation)]

    public class AvailableDeliveryDateTimeRangeMigration : ForwardOnlyMigration
    {
       

        public override void Up()
        {
            // Create the table
            Create.Table("AvailableDeliveryDateTimeRange")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("ExceptionDateId").AsInt32().Nullable()
                .WithColumn("AvailableDeliveryTimeRangeId").AsInt32().NotNullable()
                .WithColumn("DayOfWeek").AsInt32().Nullable()
                .WithColumn("StartDateOnUtc").AsDateTime2().NotNullable()
                .WithColumn("EndDateOnUtc").AsDateTime2().Nullable();

            // Create foreign key constraints
            Create.ForeignKey("FK_AvailableDeliveryDateTimeRange_AvailableDeliveryTimeRangeId_AvailableDeliveryTimeRange_Id")
                .FromTable("AvailableDeliveryDateTimeRange").ForeignColumn("AvailableDeliveryTimeRangeId")
                .ToTable("AvailableDeliveryTimeRange").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);

        }
    }

    [NopSchemaMigration("2023/09/12 05:03:00:0000022", "DeliveryScheduling base schema,DeliveryOrderDetailsMigration", MigrationProcessType.Installation)]

    public class DeliveryOrderDetailsMigration : ForwardOnlyMigration
    {
        

        public override void Up()
        {
            // Create the table
            Create.Table("DeliveryOrderDetails")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("CustomerId").AsInt32().NotNullable()
                .WithColumn("StoreId").AsInt32().NotNullable()
                .WithColumn("OrderId").AsInt32().Nullable()
                .WithColumn("Deleted").AsBoolean().NotNullable()
                .WithColumn("CreatedOnUtc").AsDateTime2().NotNullable()
                .WithColumn("UpdatedOnUtc").AsDateTime2().NotNullable();
        }
    }

    [NopSchemaMigration("2023/09/12 05:04:00:0000033", "DeliveryScheduling base schema,DeliveryOrderItemDetailsMigration", MigrationProcessType.Installation)]
    
    public class DeliveryOrderItemDetailsMigration : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            // Create the table
            Create.Table("DeliveryOrderItemDetails")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("CartId").AsInt32().NotNullable()
                .WithColumn("ShoppingCartItemId").AsInt32().NotNullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            // Add foreign key constraints
            Create.ForeignKey("FK_DeliveryOrderItemDetails_CartId_DeliveryOrderDetails_Id")
                .FromTable("DeliveryOrderItemDetails").ForeignColumn("CartId")
                .ToTable("DeliveryOrderDetails").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }
    }

    [NopSchemaMigration("2023/09/12 05:05:00:0000044", "DeliveryScheduling base schema,ShippingMethodCapacityMigration", MigrationProcessType.Installation)]

    public class ShippingMethodCapacityMigration : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            // Create the table
            Create.Table("ShippingMethodCapacity")  // This table maps to ShippingMethodCapacity
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("AvailableDeliveryDateTimeRangeId").AsInt32().Nullable()
                .WithColumn("ShippingMethodId").AsInt32().NotNullable()
                .WithColumn("Capacity").AsInt32().NotNullable()
                .WithColumn("Deleted").AsBoolean().NotNullable();

            // Add foreign key constraint
            Create.ForeignKey("FK_ShippingMethodCapacity_AvailableDeliveryDateTimeRangeId_AvailableDeliveryDateTimeRange_Id")
                .FromTable("ShippingMethodCapacity").ForeignColumn("AvailableDeliveryDateTimeRangeId")
                .ToTable("AvailableDeliveryDateTimeRange").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }
    }

    [NopSchemaMigration("2023/09/12 05:06:00:0000055", "DeliveryScheduling base schema,OrderShippingMethodCapacityMappingMigration", MigrationProcessType.Installation)]
  
    public class OrderShippingMethodCapacityMappingMigration : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            // Create the table
            Create.Table("OrderShippingMethodCapacityMapping")  // This table maps to OrderShippingMethodCapacityMapping
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("ShippingMethodCapacityId").AsInt32().Nullable()
                .WithColumn("OrderId").AsInt32().NotNullable()
                .WithColumn("DeliveryDateOnUtc").AsDateTime2().NotNullable();

            // Add foreign key constraint
            Create.ForeignKey("FK_OrderShippingMethodCapacityMapping_ShippingMethodCapacityId_ShippingMethodCapacity_Id")
                .FromTable("OrderShippingMethodCapacityMapping").ForeignColumn("ShippingMethodCapacityId")
                .ToTable("ShippingMethodCapacity").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }
    }

    [NopSchemaMigration("2023/09/12 05:01:00:0000066", "DeliveryScheduling base schema,    public class AvailableDeliveryTimeRangeMigration : AutoReversingMigration\r\n", MigrationProcessType.Installation)]
    
    public class AvailableDeliveryTimeRangeMigration : ForwardOnlyMigration
    {
        

        public override void Up()
        {
            Create.Table("AvailableDeliveryTimeRange")
           .WithColumn("Id").AsInt32().Identity().PrimaryKey()
           .WithColumn("Time").AsString(int.MaxValue).Nullable()
           .WithColumn("StoreId").AsInt32().NotNullable()
           .WithColumn("DisplayOrder").AsInt32().NotNullable()
           .WithColumn("Deleted").AsBoolean().NotNullable();
        }
    }
}
