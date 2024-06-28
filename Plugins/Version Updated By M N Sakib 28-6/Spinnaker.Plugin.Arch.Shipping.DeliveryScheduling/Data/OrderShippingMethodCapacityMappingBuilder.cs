using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Data
{
    public class OrderShippingMethodCapacityMappingBuilder : NopEntityBuilder<OrderShippingMethodCapacityMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.ShippingMethodCapacityId)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderShippingMethodCapacityMapping.DeliveryDateOnUtc)).AsDateTime2().NotNullable();
        }
    }
}
