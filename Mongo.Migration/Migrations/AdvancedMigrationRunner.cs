using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class AdvancedMigrationRunner : IAdvancedMigrationRunner
    {
        private ILogger _logger;
        public IAdvancedMigrationLocator _migrationLocator { get; }

        private readonly IVersionService _versionService;

        private IMongoDatabase _db;

        private IMongoCollection<BsonDocument> _migrationshistory;

        public AdvancedMigrationRunner(IAdvancedMigrationLocator migrationLocator, IVersionService versionService)
        {
            _migrationLocator = migrationLocator;
            _versionService = versionService;
            _logger = ApplicationLogging.CreateLogger();
        }

        public void Run(IMongoDatabase db, string runningVersion)
        {
            _logger.LogInformation($"Starting the migrations ...");

            _db = db;
            _migrationshistory = db.GetCollection<BsonDocument>("_migrationshistory");

            var migrations = _migrationLocator.GetMigrations(typeof(AdvancedMigration)) ?? Enumerable.Empty<IAdvancedMigration>();
            var migrationsToDowngrade = new List<IAdvancedMigration>();

            foreach (var migration in migrations)
            {

                var migrationsInDb = _migrationshistory.FindSync(CreateQueryForMigration(migration.GetType().ToString())).ToList();

                if (migrationsInDb.Count() > 0)
                {
                    foreach (var document in migrationsInDb)
                    {
                        if (document["productVersion"].ToString().CompareTo(runningVersion) <= 0)
                            continue;
                        else if (document["productVersion"].ToString().CompareTo(runningVersion) > 0)
                        {
                            migrationsToDowngrade.Add(migration);
                        }
                    }
                }
                else
                {
                    if (runningVersion.CompareTo(migration.Version) >= 0)
                    {
                        try
                        {
                            _logger.LogInformation("Starting the migration Up: {0}:{1} ", migration.GetType().ToString(), migration.Version);

                            migration.Up(_db);
                            _migrationshistory.InsertOne(new BsonDocument { { "migrationId", migration.GetType().ToString() }, { "productVersion", migration.Version } });

                            _logger.LogInformation("Successful migration Up: {0}:{1} ", migration.GetType().ToString(), migration.Version);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error on migration Up {0}:{1} ", migration.GetType().ToString(), migration.Version);
                        }
                    }
                }
            }

            migrationsToDowngrade.Reverse();
            foreach (var migration in migrationsToDowngrade)
            {
                try
                {
                    _logger.LogInformation("Starting the migration Down: {0}:{1} ", migration.GetType().ToString(), migration.Version);

                    migration.Down(_db);
                    _migrationshistory.DeleteOne(Builders<BsonDocument>.Filter.Eq("migrationId", migration.GetType().ToString()));

                    _logger.LogInformation("Successful migration Down: {0}:{1} ", migration.GetType().ToString(), migration.Version);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error on migration Down {0}:{1} ", migration.GetType().ToString(), migration.Version);
                }
            }

            _logger.LogInformation($"Migrations has been done.");
        }

        private FilterDefinition<BsonDocument> CreateQueryForMigration(
            string type)
        {
            return Builders<BsonDocument>.Filter.Eq("migrationId", type);
        }
    }
}