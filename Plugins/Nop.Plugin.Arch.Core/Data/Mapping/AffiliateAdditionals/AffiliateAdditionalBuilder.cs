using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Affiliates;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.Affiliates;

namespace Nop.Plugin.Arch.Core.Data.Mapping.AffiliateAdditionals;
public class AffiliateAdditionalBuilder : NopEntityBuilder<AffiliateAdditional>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AffiliateAdditional.AffiliateId)).AsInt32().ForeignKey<Affiliate>();

    }
}
