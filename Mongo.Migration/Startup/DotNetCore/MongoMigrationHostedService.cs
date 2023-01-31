using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Startup.DotNetCore
{
    public class MongoMigrationHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;

        private readonly ILogger<MongoMigrationHostedService> _logger;

        private readonly IMongoMigration _migration;

        public MongoMigrationHostedService(IHostApplicationLifetime applicationLifetime, IMongoMigration migration, ILogger<MongoMigrationHostedService> logger)
        {
            this._applicationLifetime = applicationLifetime;
            this._logger = logger;
            this._migration = migration;
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
                this._applicationLifetime.StopApplication();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}