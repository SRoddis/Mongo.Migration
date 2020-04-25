using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
    {
        private ILogger _logger;
        private IMigrationLocator<IDatabaseMigration> _migrationLocator { get; }

        public DatabaseMigrationRunner(IMigrationLocator<IDatabaseMigration> migrationLocator)
            : this(migrationLocator, NullLoggerFactory.Instance)
        {
            _migrationLocator = migrationLocator;
        }

        private DatabaseMigrationRunner(
            IMigrationLocator<IDatabaseMigration> migrationLocator,
            ILoggerFactory loggerFactory)
        {
            _migrationLocator = migrationLocator;
            _logger = loggerFactory.CreateLogger<DatabaseMigrationRunner>();
        }

        public void Run(IMongoDatabase db, DocumentVersion runningVersion)
        {
            _logger.LogInformation($"Database migration started.");

            IMongoCollection<BsonDocument> migrationshistory  = db.GetCollection<BsonDocument>("_migrationshistory");

            var migrations = _migrationLocator.GetMigrations(typeof(DatabaseMigration)) ?? Enumerable.Empty<IDatabaseMigration>();
            var migrationsToDowngrade = new List<IDatabaseMigration>();

            foreach (var migration in migrations)
            {

                var migrationsInDb = migrationshistory.FindSync(CreateQueryForMigration(migration.GetType().ToString())).ToList();

                if (migrationsInDb.Count() > 0)
                {
                    foreach (var document in migrationsInDb)
                    {
                        if (document["productVersion"].AsString < runningVersion || document["productVersion"].AsString == runningVersion)
                            continue;
                        else if (document["productVersion"].AsString > runningVersion)
                        {
                            migrationsToDowngrade.Add(migration);
                        }
                    }
                }
                else
                {
                    if (runningVersion > migration.Version || runningVersion == migration.Version)
                    {
                        try
                        {
                            _logger.LogInformation("Database Migration Up: {0}:{1} ", migration.GetType().ToString(), migration.Version);

                            migration.Up(db);
                            migrationshistory.InsertOne(new BsonDocument { { "migrationId", migration.GetType().ToString() }, { "productVersion", migration.Version.ToString() } });

                            _logger.LogInformation("Database Migration Up finished successful: {0}:{1} ", migration.GetType().ToString(), migration.Version);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error durring migration Up execution {0}:{1} ", migration.GetType().ToString(), migration.Version);
                        }
                    }
                }
            }

            migrationsToDowngrade.Reverse();
            foreach (var migration in migrationsToDowngrade)
            {
                try
                {
                    _logger.LogInformation("Database Migration Down: {0}:{1} ", migration.GetType().ToString(), migration.Version);

                    migration.Down(db);
                    migrationshistory.DeleteOne(Builders<BsonDocument>.Filter.Eq("migrationId", migration.GetType().ToString()));

                    _logger.LogInformation("Database Migration Down finished successful: {0}:{1} ", migration.GetType().ToString(), migration.Version);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error durring migration Down execution {0}:{1} ", migration.GetType().ToString(), migration.Version);
                }
            }

            _logger.LogInformation($"Database migration finished.");
        }

        private FilterDefinition<BsonDocument> CreateQueryForMigration(
            string type)
        {
            return Builders<BsonDocument>.Filter.Eq("migrationId", type);
        }
    }
}