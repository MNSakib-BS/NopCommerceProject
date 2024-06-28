using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Catalogs;
public class CategoryAdditionalBuilder : NopEntityBuilder<CategoryAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CategoryAdditional.StoreTypeId)).AsInt32().ForeignKey<StoreType>()
            .WithColumn(nameof(CategoryAdditional.CategoryId)).AsInt32().ForeignKey<Category>();
    }
}
