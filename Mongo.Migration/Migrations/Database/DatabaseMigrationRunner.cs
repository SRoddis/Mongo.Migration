using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
    {
        private const string MigrationsCollectionName = "_migrations";
        private readonly Type DatabaseMigrationType = typeof(DatabaseMigration);
        private ILogger _logger;
        private IDatabaseTypeMigrationDependencyLocator _migrationLocator { get; }

        public DatabaseMigrationRunner(IDatabaseTypeMigrationDependencyLocator migrationLocator)
            : this(migrationLocator, NullLoggerFactory.Instance)
        {
            _migrationLocator = migrationLocator;
        }

        private DatabaseMigrationRunner(
            IDatabaseTypeMigrationDependencyLocator migrationLocator,
            ILoggerFactory loggerFactory)
        {
            _migrationLocator = migrationLocator;
            _logger = loggerFactory.CreateLogger<DatabaseMigrationRunner>();
        }

        public void Run(IMongoDatabase db, DocumentVersion databaseMigrationVersion)
        {
            _logger.LogInformation($"Database migration started.");
            var migrationsCollection = GetMigrationsCollection(db);
            var currentVersion = GetLatestOrDefaultVersion(migrationsCollection.Find(m => true).ToList());

            var currentOrLatest = _migrationLocator.GetLatestVersion(typeof(DatabaseMigration));

            if (currentVersion == currentOrLatest)
            {
                return;
            }

            MigrateUpOrDown(db, currentVersion, currentOrLatest);
        }

        private static IMongoCollection<MigrationHistory> GetMigrationsCollection(IMongoDatabase db)
        {
            return db.GetCollection<MigrationHistory>(MigrationsCollectionName);
        }

        private void MigrateUpOrDown(
            IMongoDatabase db,
            DocumentVersion currentVersion,
            DocumentVersion to)
        {
            if (currentVersion > to)
            {

                MigrateDown(db, to);
                return;
            }

            MigrateUp(db, currentVersion, to);

        }

        private void MigrateUp(IMongoDatabase db, DocumentVersion currentVersion, DocumentVersion toVersion)
        {
            var migrations = _migrationLocator.GetMigrationsFromTo(DatabaseMigrationType, currentVersion, toVersion).ToList();

            foreach (var migration in migrations)
            {
                _logger.LogInformation("Database Migration Up: {0}:{1} ", currentVersion.GetType().ToString(), migration.Version);

                migration.Up(db);
                GetMigrationsCollection(db).InsertOne(new MigrationHistory
                {
                    MigrationId = migration.GetType().ToString(),
                    Version = migration.Version
                });

                _logger.LogInformation("Database Migration Up finished successful: {0}:{1} ", migration.GetType().ToString(), migration.Version);
            }
        }

        private void MigrateDown(IMongoDatabase db, DocumentVersion version)
        {
            var migrations = _migrationLocator
                .GetMigrationsGtEq(DatabaseMigrationType, version)
                .OrderByDescending(m => m.Version)
                .ToList();

            for (var m = 0; m < migrations.Count; m++)
            {
                var migration = migrations[m];
                if (version == migration.Version)
                {
                    break;
                }

                _logger.LogInformation("Database Migration Down: {0}:{1} ", migration.GetType().ToString(), migration.Version);

                migration.Down(db);
                GetMigrationsCollection(db).DeleteOne(Builders<MigrationHistory>.Filter.Eq(mh => mh.MigrationId, migration.GetType().ToString()));

                _logger.LogInformation("Database Migration Down finished successful: {0}:{1} ", migration.GetType().ToString(), migration.Version);
            }
        }

        public DocumentVersion GetLatestOrDefaultVersion(IEnumerable<MigrationHistory> migrations)
        {
            if (migrations == null || !migrations.Any())
            {
                return DocumentVersion.Default();
            }

            return migrations.Max(m => m.Version);
        }
    }
}