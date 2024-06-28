using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Data
{
    public class ShippingMethodCapacityBuilder : NopEntityBuilder<ShippingMethodCapacity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShippingMethodCapacity.Id)).AsInt32().Identity().PrimaryKey()
                .WithColumn(nameof(ShippingMethodCapacity.AvailableDeliveryDateTimeRangeId)).AsInt32().NotNullable()
                .WithColumn(nameof(ShippingMethodCapacity.ShippingMethodId)).AsInt32().NotNullable()
                .WithColumn(nameof(ShippingMethodCapacity.Capacity)).AsInt32().NotNullable()
                .WithColumn(nameof(ShippingMethodCapacity.Deleted)).AsBoolean().NotNullable();
        }
    }
}
