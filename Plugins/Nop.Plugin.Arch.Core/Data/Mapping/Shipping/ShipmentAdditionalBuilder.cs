using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Shipping;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Shipping;
public class ShipmentAdditionalBuilder : NopEntityBuilder<ShipmentAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ShipmentAdditional.ShipmentId)).AsInt32().ForeignKey<Shipment>();
    }
}
