using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Migrations.Locators;

internal class TypeMigrationDependencyLocator<TMigrationType> : MigrationLocator<TMigrationType>
    where TMigrationType : class, IMigration
{
    public TypeMigrationDependencyLocator(ILogger<TypeMigrationDependencyLocator<TMigrationType>> logger, IServiceProvider serviceProvider)
        : base(logger, serviceProvider) { }
}