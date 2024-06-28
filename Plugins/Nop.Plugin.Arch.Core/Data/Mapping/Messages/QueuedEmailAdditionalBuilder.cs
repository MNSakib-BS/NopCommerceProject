using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Messages;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Messages;
public class QueuedEmailAdditionalBuilder : NopEntityBuilder<QueuedEmailAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
             .WithColumn(nameof(QueuedEmailAdditional.QueuedEmailId)).AsInt32().ForeignKey<QueuedEmail>();
    }
}
