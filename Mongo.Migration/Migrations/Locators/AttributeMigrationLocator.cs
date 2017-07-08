using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Migrations.Attributes;

namespace Mongo.Migration.Migrations.Locators
{
    internal class AttributeMigrationLocator : MigrationLocator
    {
        protected override IEnumerable<IMigration> LoadMigrations()
        {
            var typesList = AppDomain.CurrentDomain.GetAssemblies().Select(
                assembly => from type in assembly.GetTypes()
                    where Attribute.IsDefined(type, typeof(MigrationMaker))
                    select type
            );

            var instances = new List<IMigration>();

            foreach (var types in typesList)
            {
                var migrations = types.Select(Activator.CreateInstance)
                    .OfType<IMigration>();

                instances.AddRange(migrations);
            }

            return instances;
        }
    }
}