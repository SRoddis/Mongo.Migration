using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal class  DatabaseMigrationRunner : IDatabaseMigrationRunner
    {
        private readonly IMongoClient _client;

        private readonly IAdvancedMigrationRunner _migrationRunner;

        private readonly string _databaseName;

        private readonly string _runningVersion;

        private readonly IOptions<MongoMigrationSettings> _options;
        
        private const int CONNECTION_CHECK_TIMEOUT = 1000;
        public DatabaseMigrationRunner(
            IOptions<MongoMigrationSettings> options,
            IAdvancedMigrationRunner migrationRunner)
            : this(
                new MongoClient(options.Value.ConnectionString),
                migrationRunner,
                options.Value.RunningVersion)
        {
            _options = options;
            _databaseName = options.Value.Database;
        }

        public DatabaseMigrationRunner(
            IMongoClient client,
            IAdvancedMigrationRunner migrationRunner,
            string runningVersion)
        {
            _runningVersion = runningVersion;
            _client = client;
            _migrationRunner = migrationRunner;
        }

        public void RunAll()
        {
            _migrationRunner.Run(_client.GetDatabase(_databaseName), _runningVersion);
        }
    }
}