using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.OurTeam.Domain;

namespace Nop.Plugin.Widgets.OurTeam.Migrations;

[NopSchemaMigration("2024/06/10 08:40:55:1687541", "OurTeam.Employeessss base schema", MigrationProcessType.Installation)]

public class SchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.TableFor<Employees>();
    }
}
