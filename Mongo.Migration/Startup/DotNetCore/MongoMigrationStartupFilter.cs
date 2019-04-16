using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mongo.Migration.Startup.DotNetCore
{
    public class MongoMigrationStartupFilter : IStartupFilter
    {
        private readonly IMongoMigration _migration;
        private readonly ILogger<MongoMigrationStartupFilter> _logger;

        public MongoMigrationStartupFilter(IMongoMigration migration)
            : this(migration, NullLoggerFactory.Instance)
        {
            
        }

        public MongoMigrationStartupFilter(IMongoMigration migration, ILoggerFactory loggerFactory)
        {
            _migration = migration;
            _logger = loggerFactory.CreateLogger<MongoMigrationStartupFilter>();
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            try
            {
                _migration.Run();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetType().ToString());
            }

            return next;
        }
    }
}