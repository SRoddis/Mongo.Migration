using System;
using System.Collections.Generic;
using Mongo.Migration.Documents;

namespace Mongo.Migration.Migrations.Locators
{
    public interface IMigrationLocator
    {
        IEnumerable<IMigration> GetMigrations(Type type);

        IEnumerable<IMigration> GetMigrationsLte(Type type, DocumentVersion version);

        IEnumerable<IMigration> GetMigrationsGt(Type type, DocumentVersion version);

        DocumentVersion GetLatestVersion(Type type);
    }
}