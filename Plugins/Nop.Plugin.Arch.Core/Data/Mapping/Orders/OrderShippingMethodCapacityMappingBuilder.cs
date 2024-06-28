using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Data.Extensions;
using Nop.Core.Domain.Orders;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class OrderShippingMethodCapacityMappingBuilder : NopEntityBuilder<OrderShippingMethodCapacityMapping>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
         table
            .WithColumn(nameof(OrderShippingMethodCapacityMapping.ShippingMethodCapacityId)).AsInt32().ForeignKey<RL_DS_ShippingMethodCapacity>()
            .WithColumn(nameof(OrderShippingMethodCapacityMapping.OrderId)).AsInt32().ForeignKey<Order>();
    }
}
