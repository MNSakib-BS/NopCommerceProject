using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Customers;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Customers;
public class CustomerPictureBuilder : NopEntityBuilder<CustomerPicture>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerPicture.CustomerId)).AsInt32().ForeignKey<Customer>();
    }
}
