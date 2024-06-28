using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Plugin.Arch.Core.Domains.Promotions;
using Nop.Data.Extensions;


namespace Nop.Plugin.Arch.Core.Data.Mapping.Promotions;
public class PromotionGroupWidgetZonesBuilder : NopEntityBuilder<PromotionGroupWidgetZones>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PromotionGroupWidgetZones.PromotionGroupId)).AsInt32().ForeignKey<PromotionGroup>();
    }
}
