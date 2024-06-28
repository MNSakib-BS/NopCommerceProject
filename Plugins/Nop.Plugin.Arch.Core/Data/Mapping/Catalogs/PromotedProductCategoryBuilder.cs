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

namespace Nop.Plugin.Arch.Core.Data.Mapping.Catalogs;
public class PromotedProductCategoryBuilder : NopEntityBuilder<PromotedProductCategory>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PromotedProductCategory.ProductId)).AsInt32().ForeignKey<Product>()
            .WithColumn(nameof(PromotedProductCategory.CategoryId)).AsInt32().ForeignKey<Category>();
    }
}
