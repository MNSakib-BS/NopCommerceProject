using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.Messages;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Messages;
public class EmailAccountAdditionalBuilder : NopEntityBuilder<EmailAccountAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(EmailAccountAdditional.EmailAccountId)).AsInt32().ForeignKey<EmailAccount>();
    }
}
