using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.NopstationTeamTest.Domain;

namespace Nop.Plugin.Misc.NopstationTeamTest.Migrations;

[NopSchemaMigration("2024/05/27 08:40:55:1687541", "NopstationTeamTest.Employee base schema", MigrationProcessType.Installation)]

public class SchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.TableFor<Employee>();
    }
}
