using System;
using System.Collections.Generic;
using Mongo.Migration.Documents;

namespace Mongo.Migration.Migrations.Locators
{
    public interface IAdvancedMigrationLocator
    {
        IEnumerable<IAdvancedMigration> GetMigrations(Type type);

        IEnumerable<IAdvancedMigration> GetMigrationsGt(Type type, string version);

        IEnumerable<IAdvancedMigration> GetMigrationsGtEq(Type type, string version);

        DocumentVersion GetLatestVersion(Type type);

        void Locate();
    }
}