using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;

namespace Nop.Plugin.Arch.Core.Data.Mapping.Catalogs;
public class ArchStoreProductInfoBuilder : NopEntityBuilder<ArchStoreProductInfo>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
             .WithColumn(nameof(ArchStoreProductInfo.StoreTypeId)).AsInt32().ForeignKey<StoreType>()
             .WithColumn(nameof(ArchStoreProductInfo.StoreId)).AsInt32().ForeignKey<Store>()
             .WithColumn(nameof(ArchStoreProductInfo.ProductId)).AsInt32().ForeignKey<Product>();
    }
}
