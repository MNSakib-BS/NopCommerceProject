using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Data.Extensions;
using Nop.Core.Domain.Shipping;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class RL_DS_ShippingMethodCapacityBuilder : NopEntityBuilder<RL_DS_ShippingMethodCapacity>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
             .WithColumn(nameof(RL_DS_ShippingMethodCapacity.ShippingMethodId)).AsInt32().ForeignKey<ShippingMethod>();
    }
}
