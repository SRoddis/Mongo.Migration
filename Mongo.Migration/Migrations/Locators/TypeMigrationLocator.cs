using System;
using System.Linq;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Document;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationLocator : MigrationLocator<IDocumentMigration>
    {
        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                 from type in assembly.GetTypes()
                 where typeof(IDocumentMigration).IsAssignableFrom(type) && !type.IsAbstract
                 select type).Distinct();

            Migrations = migrationTypes.Select(t => (IDocumentMigration)Activator.CreateInstance(t)).ToMigrationDictionary();
        }
    }
}