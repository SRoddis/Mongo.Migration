using System;
using System.Linq;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    internal class AdvancedTypeMigrationLocator : AdvancedMigrationLocator
    {
        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                from type in assembly.GetTypes()
                where typeof(IAdvancedMigration).IsAssignableFrom(type) && !type.IsAbstract
                select type).Distinct();

            Migrations = migrationTypes.Select(t => (IAdvancedMigration) Activator.CreateInstance(t)).ToMigrationDictionary();
        }
    }
}