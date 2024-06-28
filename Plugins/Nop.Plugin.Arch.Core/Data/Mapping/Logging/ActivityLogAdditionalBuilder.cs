using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.Logging;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Logging;
public class ActivityLogAdditionalBuilder : NopEntityBuilder<ActivityLogAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ActivityLogAdditional.ActivityLogId)).AsInt32().ForeignKey<ActivityLog>();
    }
}
