using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Customers;
public class CustomerAttributeAdditionalBuilder : NopEntityBuilder<CustomerAttributeAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerAttributeAdditional.CustomerAttributeId)).AsInt32().ForeignKey<CustomerAttribute>();
    }
}
