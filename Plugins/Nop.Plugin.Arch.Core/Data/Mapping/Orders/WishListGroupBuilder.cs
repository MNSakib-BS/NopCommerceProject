using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Data.Extensions;
using Nop.Core.Domain.Stores;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class WishListGroupBuilder : NopEntityBuilder<WishListGroup>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(WishListGroup.StoreId)).AsInt32().ForeignKey<Store>()
            .WithColumn(nameof(WishListGroup.CustomerId)).AsInt32().ForeignKey<Customer>();
    }
}
