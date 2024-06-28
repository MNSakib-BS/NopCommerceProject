using Nop.Data.Mapping.Builders;
using FluentMigrator.Builders.Create.Table;
using Nop.Plugin.NopStation.MegaMenu.Domains;

namespace Nop.Plugin.NopStation.MegaMenu.Data
{
    public class CategoryIconRecordBuilder : NopEntityBuilder<CategoryIcon>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
              .WithColumn(nameof(CategoryIcon.CategoryId))
              .AsInt32()
              .WithColumn(nameof(CategoryIcon.PictureId))
              .AsInt32();
        }
    }
}
