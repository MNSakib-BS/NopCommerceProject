using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.NopStation.MegaMenu.Domains;

namespace Nop.Plugin.NopStation.MegaMenu.Data
{
    [NopSchemaMigration("2020/09/20 03:10:23:1264924", "NopStation.MegaMenu base schema\"", MigrationProcessType.Installation)]

    public class SchemaMigration : ForwardOnlyMigration
    {
        
        public override void Up()
        {
            Create.TableFor<CategoryIcon>();
        }
    }
}
