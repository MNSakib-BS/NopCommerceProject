using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Promotions;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Promotions;
public class ProductPromotionGroupBuilder : NopEntityBuilder<ProductPromotionGroup>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ProductPromotionGroup.ProductId)).AsInt32().ForeignKey<Product>()
            .WithColumn(nameof(ProductPromotionGroup.PromotionGroupId)).AsInt32().ForeignKey<PromotionGroup>();
    }
}
