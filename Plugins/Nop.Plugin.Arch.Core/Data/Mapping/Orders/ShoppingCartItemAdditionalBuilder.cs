using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class ShoppingCartItemAdditionalBuilder : NopEntityBuilder<ShoppingCartItemAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ShoppingCartItemAdditional.ShoppingCartItemId)).AsInt32().ForeignKey<ShoppingCartItem>();
    }
}
