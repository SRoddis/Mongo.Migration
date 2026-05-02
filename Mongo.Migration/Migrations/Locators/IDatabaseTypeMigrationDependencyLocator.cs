using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Migrations.Locators;

internal interface IDatabaseTypeMigrationDependencyLocator : IMigrationLocator<IDatabaseMigration>;