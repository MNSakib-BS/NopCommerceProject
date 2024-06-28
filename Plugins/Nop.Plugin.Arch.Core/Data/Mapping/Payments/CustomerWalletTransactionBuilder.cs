using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Payments;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Payments;
public class CustomerWalletTransactionBuilder : NopEntityBuilder<CustomerWalletTransaction>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerWalletTransaction.CustomerWalletId)).AsInt32().ForeignKey<CustomerWallet>();
    }
}
