using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.OurTeam.Domain;

namespace Nop.Plugin.Widgets.OurTeam.Mapping.Builders
{
    public class EmployeesBuilder : NopEntityBuilder<Employees>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Employees.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Employees.Name)).AsString().NotNullable()
                .WithColumn(nameof(Employees.Designation)).AsString().NotNullable()
                .WithColumn(nameof(Employees.IsMVP)).AsBoolean().NotNullable()
                .WithColumn(nameof(Employees.IsNopCommerceCertified)).AsBoolean().NotNullable();
        }
    }
}
