using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class AvailableDeliveryTimeRangeBuilder : NopEntityBuilder<AvailableDeliveryTimeRange>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AvailableDeliveryTimeRange.StoreId)).AsInt32().ForeignKey<Store>();
    
    }
}
