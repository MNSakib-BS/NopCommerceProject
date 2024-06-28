using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.Gdpr;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Gdpr;
public class GdprLogAdditionalBuilder : NopEntityBuilder<GdprLogAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(GdprLogAdditional.GdprLogId)).AsInt32().ForeignKey<GdprLog>();
    }
}
