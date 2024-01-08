using System;
using System.Linq;

using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Document;

namespace Mongo.Migration.Migrations.Locators;

internal class TypeMigrationLocator : MigrationLocator<IDocumentMigration>
{
    public override void Locate()
    {
        var migrationTypes =
            (from assembly in this.Assemblies
             from type in assembly.GetTypes()
             where typeof(IDocumentMigration).IsAssignableFrom(type) && !type.IsAbstract
             select type).Distinct();

        this.Migrations = migrationTypes.Select(t => (IDocumentMigration)Activator.CreateInstance(t)).ToMigrationDictionary();
    }
}