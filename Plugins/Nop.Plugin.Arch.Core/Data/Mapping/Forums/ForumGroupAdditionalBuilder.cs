﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Domains.Forums;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Forums;
public class ForumGroupAdditionalBuilder : NopEntityBuilder<ForumGroupAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
             .WithColumn(nameof(ForumGroupAdditional.ForumGroupId)).AsInt32().ForeignKey<ForumGroup>();
    }
}
