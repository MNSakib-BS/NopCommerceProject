using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Stores;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Stores;
public class StoreAdditionalBuilder : NopEntityBuilder<StoreAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(StoreAdditional.StoreId)).AsInt32().ForeignKey<Store>();
    }
}
