using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.Discounts;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Discounts;
public class DiscountAdditionalBuilder : NopEntityBuilder<DiscountAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(DiscountAdditional.DiscountId)).AsInt32().ForeignKey<Discount>();
    }
}
