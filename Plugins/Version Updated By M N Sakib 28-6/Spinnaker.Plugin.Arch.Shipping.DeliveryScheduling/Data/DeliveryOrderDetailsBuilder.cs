using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Data
{
    public class DeliveryOrderDetailsBuilder : NopEntityBuilder<DeliveryOrderDetails>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DeliveryOrderDetails.Id)).AsInt32().Identity().PrimaryKey()
                .WithColumn(nameof(DeliveryOrderDetails.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(DeliveryOrderDetails.StoreId)).AsInt32().NotNullable()
                .WithColumn(nameof(DeliveryOrderDetails.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(DeliveryOrderDetails.Deleted)).AsBoolean().NotNullable()
                .WithColumn(nameof(DeliveryOrderDetails.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(DeliveryOrderDetails.UpdatedOnUtc)).AsDateTime2().NotNullable();
        }
    }
}
