﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class RL_DS_AvailableDeliveryDateTimeRangeBuilder : NopEntityBuilder<RL_DS_AvailableDeliveryDateTimeRange>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
         table
            .WithColumn(nameof(RL_DS_AvailableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId)).AsInt32().ForeignKey<AvailableDeliveryTimeRange>();
    }
}
