using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Data
{
    public class AvailableDeliveryDateTimeRangeBuilder : NopEntityBuilder<AvailableDeliveryDateTimeRange>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.Id)).AsInt32().Identity().PrimaryKey()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.ExceptionDateId)).AsInt32().Nullable()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId)).AsInt32().NotNullable()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.DayOfWeek)).AsInt32().NotNullable()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.StartDateOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(AvailableDeliveryDateTimeRange.EndDateOnUtc)).AsDateTime2().NotNullable();
        }
    }
}
