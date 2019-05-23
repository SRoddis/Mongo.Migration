using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mongo.Migration.Startup.DotNetCore
{
    public class MongoMigrationStartupFilter : IStartupFilter
    {
        private readonly IMongoMigration _migration;
        private readonly ILogger<MongoMigrationStartupFilter> _logger;

        public MongoMigrationStartupFilter(IServiceScopeFactory serviceScopeFactory)
            : this(serviceScopeFactory, NullLoggerFactory.Instance)
        {
        }

        public MongoMigrationStartupFilter(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
        {
            _migration = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IMongoMigration>();
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