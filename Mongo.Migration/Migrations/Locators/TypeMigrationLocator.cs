using System;
using System.Collections.Generic;
using System.Linq;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationLocator : MigrationLocator
    {
        public override IList<IMigration> LoadMigrations()
        {
            var migrationTypes =
                (from assembly in Assemblies
                from type in assembly.GetTypes()
                where typeof(IMigration).IsAssignableFrom(type) && !type.IsAbstract
                select type).Distinct();

            return migrationTypes.Select(t => (IMigration) Activator.CreateInstance(t)).ToList();
        }
    }
}