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
using Nop.Plugin.Arch.Core.Domains.Payments;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Stores;
public class CustomerWalletStoreMappingBuilder : NopEntityBuilder<CustomerWalletStoreMapping>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerWalletStoreMapping.StoreId)).AsInt32().ForeignKey<Store>()
            .WithColumn(nameof(CustomerWalletStoreMapping.CustomerWalletId)).AsInt32().ForeignKey<CustomerWallet>();
    }
}
