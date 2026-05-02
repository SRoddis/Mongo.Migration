using Mongo.Migration.Documents;

namespace Mongo.Migration.Migrations.Locators;

public interface IMigrationLocator<out TMigrationType>
    where TMigrationType : class, IMigration
{
    IReadOnlyCollection<TMigrationType> GetMigrations(Type type);

    IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version, DocumentVersion otherVersion);

    IEnumerable<TMigrationType> GetMigrationsFromToDown(Type type, DocumentVersion version, DocumentVersion otherVersion);

    DocumentVersion GetLatestVersion(Type type);

    void Initialize();
}