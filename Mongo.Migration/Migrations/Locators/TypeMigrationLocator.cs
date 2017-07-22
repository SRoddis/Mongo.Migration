using System;
using System.Linq;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationLocator : MigrationLocator
    {
        public override void LoadMigrations()
        {
            var migrationTypes =
                (from assembly in Assemblies
                from type in assembly.GetTypes()
                where typeof(IMigration).IsAssignableFrom(type) && !type.IsAbstract
                select type).Distinct();

            Migrations = migrationTypes.Select(t => (IMigration) Activator.CreateInstance(t)).ToMigrationDictionary();
        }
    }
}