using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Data
{
    public class DeliveryOrderItemDetailsBuilder : NopEntityBuilder<DeliveryOrderItemDetails>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DeliveryOrderItemDetails.Id)).AsInt32().Identity().PrimaryKey()
                .WithColumn(nameof(DeliveryOrderItemDetails.CartId)).AsInt32().NotNullable()
                .WithColumn(nameof(DeliveryOrderItemDetails.ShoppingCartItemId)).AsInt32().NotNullable()
                .WithColumn(nameof(DeliveryOrderItemDetails.Deleted)).AsBoolean().NotNullable();
        }
    }
}
