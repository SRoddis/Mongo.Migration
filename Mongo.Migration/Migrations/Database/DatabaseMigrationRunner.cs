using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
    {
        private readonly IDatabaseVersionService _databaseVersionService;

        private readonly ILogger _logger;

        private readonly Type DatabaseMigrationType = typeof(DatabaseMigration);

        private IDatabaseTypeMigrationDependencyLocator _migrationLocator { get; }

        public DatabaseMigrationRunner(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            IDatabaseVersionService databaseVersionService)
            : this(migrationLocator, databaseVersionService, NullLoggerFactory.Instance)
        {
        }

        private DatabaseMigrationRunner(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            IDatabaseVersionService databaseVersionService,
            ILoggerFactory loggerFactory)
        {
            this._migrationLocator = migrationLocator;
            this._databaseVersionService = databaseVersionService;
            this._logger = loggerFactory.CreateLogger<DatabaseMigrationRunner>();
        }

        public void Run(IMongoDatabase db)
        {
            this._logger.LogInformation("Database migration started.");
            var databaseVersion = this._databaseVersionService.GetLatestDatabaseVersion(db);
            var currentOrLatest = this._databaseVersionService.GetCurrentOrLatestMigrationVersion();

            if (databaseVersion == currentOrLatest)
            {
                return;
            }

            this.MigrateUpOrDown(db, databaseVersion, currentOrLatest);
            this._logger.LogInformation("Database migration finished.");
        }

        private void MigrateUpOrDown(
            IMongoDatabase db,
            DocumentVersion databaseVersion,
            DocumentVersion to)
        {
            if (databaseVersion > to)
            {
                this.MigrateDown(db, databaseVersion, to);
                return;
            }

            this.MigrateUp(db, databaseVersion, to);
        }

        private void MigrateUp(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
        {
            var migrations = this._migrationLocator.GetMigrationsFromTo(this.DatabaseMigrationType, currentVersion, toVersion).ToList();

            foreach (var migration in migrations)
            {
                this._logger.LogInformation("Database Migration Up: {0}:{1} ", currentVersion.GetType().ToString(), migration.Version);

                migration.Up(db);
                this._databaseVersionService.Save(db, migration);

                this._logger.LogInformation("Database Migration Up finished successful: {0}:{1} ", migration.GetType().ToString(), migration.Version);
            }
        }

        private void MigrateDown(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
        {
            var migrations = this._migrationLocator
                .GetMigrationsGtEq(this.DatabaseMigrationType, toVersion)
                .OrderByDescending(m => m.Version)
                .ToList();

            for (var m = 0; m < migrations.Count; m++)
            {
                var migration = migrations[m];
                if (migration.Version == toVersion)
                {
                    break;
                }

                this._logger.LogInformation("Database Migration Down: {0}:{1} ", migration.GetType().ToString(), migration.Version);

                migration.Down(db);
                this._databaseVersionService.Remove(db, migration);

                this._logger.LogInformation("Database Migration Down finished successful: {0}:{1} ", migration.GetType().ToString(), migration.Version);
            }
        }
    }
}