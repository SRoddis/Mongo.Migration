using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Startup.DotNetCore
{
    public class MongoMigrationHostedService : IHostedService
    {
        private readonly ILogger<MongoMigrationHostedService> _logger;

        private readonly IMongoMigration _migration;

        public MongoMigrationHostedService(
            IMongoMigration migration,
            ILogger<MongoMigrationHostedService> logger)
        {
            this._migration = migration;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                this._logger.LogInformation("Running migration. Please wait....");
                this._migration.Run();
                this._logger.LogInformation("Migration has been done");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.GetType().ToString());
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}