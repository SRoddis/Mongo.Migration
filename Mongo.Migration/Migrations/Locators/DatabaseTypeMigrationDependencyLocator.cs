using Microsoft.Extensions.Logging;
using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Migrations.Locators;

internal class DatabaseTypeMigrationDependencyLocator : TypeMigrationDependencyLocator<IDatabaseMigration>, IDatabaseTypeMigrationDependencyLocator
{
    public DatabaseTypeMigrationDependencyLocator(ILogger<DatabaseTypeMigrationDependencyLocator> logger, IServiceProvider serviceProvider)
        : base(logger, serviceProvider) { }
}