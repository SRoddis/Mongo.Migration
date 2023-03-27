using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mongo.Migration.Startup;

public class MongoMigrationHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _applicationLifetime;

    private readonly ILogger<MongoMigrationHostedService> _logger;

    private readonly IMongoMigration _migration;

    public MongoMigrationHostedService(IHostApplicationLifetime applicationLifetime, IMongoMigration migration, ILogger<MongoMigrationHostedService> logger)
    {
        _applicationLifetime = applicationLifetime;
        _logger = logger;
        _migration = migration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Running migration. Please wait....");
            _migration.Run();
            _logger.LogInformation("Migration has been done");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.GetType().ToString());
            _applicationLifetime.StopApplication();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}