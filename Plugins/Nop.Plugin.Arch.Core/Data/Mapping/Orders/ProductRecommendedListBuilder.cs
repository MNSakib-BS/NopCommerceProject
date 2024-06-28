using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Data.Extensions;
using Nop.Plugin.Arch.Core.Domains.Catalog;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Orders;
public class ProductRecommendedListBuilder : NopEntityBuilder<ProductRecommendedList>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
       table
            .WithColumn(nameof(ProductRecommendedList.ProductId)).AsInt32().ForeignKey<Product>()
            .WithColumn(nameof(ProductRecommendedList.RecommendedListId)).AsInt32().ForeignKey<RecommendedList>();
    }
}
