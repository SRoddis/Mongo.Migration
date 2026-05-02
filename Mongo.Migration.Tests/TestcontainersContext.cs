using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Driver;
using NUnit.Framework;
using Testcontainers.MongoDb;

namespace Mongo.Migration.Tests;

[SetUpFixture]
public sealed class TestcontainersContext
{
    private static readonly Lazy<MongoDbContainer> s_lazyMongoDbContainer = new(() => new MongoDbBuilder("mongo:8").Build());

    private static ServiceProvider? s_provider;

    public static IServiceProvider Provider => s_provider ?? throw new InvalidOperationException("Must be setup");

    public static IMongoClient MongoClient => Provider.GetRequiredService<IMongoClient>();


    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await s_lazyMongoDbContainer.Value.StartAsync();

        IServiceCollection services = new ServiceCollection();
        services
            .AddLogging(builder => builder.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(new MongoClient(s_lazyMongoDbContainer.Value.GetConnectionString()))
            .AddMigration(cfg =>
            {
                cfg
                    .AddDocumentMigratedType<TestClassWithTwoMigrationMiddleVersion>("0.0.1");

                cfg.AddRuntimeDocumentMigration()
                    .AddStartupDocumentMigration()
                    .AddDatabaseMigration();
            });

        s_provider = services.BuildServiceProvider();

        IMigrationService migrationService = s_provider.GetRequiredService<IMigrationService>();
        migrationService.RegisterBsonStatics();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (s_provider is not null)
        {
            await s_provider.DisposeAsync();
        }
        
        await s_lazyMongoDbContainer.Value.StopAsync();
        await s_lazyMongoDbContainer.Value.DisposeAsync();
    }
}