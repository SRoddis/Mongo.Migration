using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Bson;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Startup;

[TestFixture]
internal class MigrationBuilderTests
{
    [Test]
    public void AllMigrationsEnabledWhenNotSet()
    {
        IServiceCollection services = CreateEmptyServiceCollection();

        services.AddMigration();

        using ServiceProvider provider = services.BuildServiceProvider();

        MongoMigrationStartupSettings startupSettings = provider.GetRequiredService<MongoMigrationStartupSettings>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(startupSettings.RuntimeMigrationEnabled, Is.True);
            Assert.That(startupSettings.DatabaseMigrationEnabled, Is.True);
            Assert.That(startupSettings.StartupDocumentMigrationEnabled, Is.True);
        }
    }

    [Test]
    public void RuntimeMigrationOnlyRequiredRegistered()
    {
        IServiceCollection services = CreateEmptyServiceCollection();

        services.AddMigration(builder => builder.AddRuntimeDocumentMigration());

        using ServiceProvider provider = services.BuildServiceProvider();

        MongoMigrationStartupSettings startupSettings = provider.GetRequiredService<MongoMigrationStartupSettings>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(startupSettings.RuntimeMigrationEnabled, Is.True);
            Assert.That(startupSettings.DatabaseMigrationEnabled, Is.False);
            Assert.That(startupSettings.StartupDocumentMigrationEnabled, Is.False);

            Assert.That(provider.GetRequiredService<IMigrationService>(), Is.TypeOf<MigrationService>());
            Assert.DoesNotThrow(() => provider.GetRequiredService<MigrationBsonSerializerProvider>());

            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<ICollectionLocator>());
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IStartUpDocumentMigrationRunner>());
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IDatabaseMigrationRunner>());
        }
    }

    [Test]
    public void DatabaseMigrationOnlyRequiredRegistered()
    {
        IServiceCollection services = CreateEmptyServiceCollection();

        services.AddMigration(builder => builder.AddDatabaseMigration());

        using ServiceProvider provider = services.BuildServiceProvider();

        MongoMigrationStartupSettings startupSettings = provider.GetRequiredService<MongoMigrationStartupSettings>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(startupSettings.RuntimeMigrationEnabled, Is.False);
            Assert.That(startupSettings.DatabaseMigrationEnabled, Is.True);
            Assert.That(startupSettings.StartupDocumentMigrationEnabled, Is.False);

            Assert.That(
                provider.GetRequiredService<IMigrationService>(),
                Is.TypeOf<MigrationService>());
            Assert.That(
                provider.GetRequiredService<IDatabaseMigrationRunner>(),
                Is.TypeOf<DatabaseMigrationRunner>());

            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<MigrationBsonSerializerProvider>());
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<ICollectionLocator>());
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IStartUpDocumentMigrationRunner>());
        }
    }

    [Test]
    public void DocumentStartupMigrationOnlyRequiredRegistered()
    {
        IServiceCollection services = CreateEmptyServiceCollection();

        services.AddMigration(builder => builder.AddStartupDocumentMigration());

        using ServiceProvider provider = services.BuildServiceProvider();

        MongoMigrationStartupSettings startupSettings = provider.GetRequiredService<MongoMigrationStartupSettings>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(startupSettings.RuntimeMigrationEnabled, Is.False);
            Assert.That(startupSettings.DatabaseMigrationEnabled, Is.False);
            Assert.That(startupSettings.StartupDocumentMigrationEnabled, Is.True);

            Assert.That(
                provider.GetRequiredService<IMigrationService>(),
                Is.TypeOf<MigrationService>());
            Assert.That(
                provider.GetRequiredService<IStartUpDocumentMigrationRunner>(),
                Is.TypeOf<StartUpDocumentMigrationRunner>());

            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<MigrationBsonSerializerProvider>());
            Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IDatabaseMigrationRunner>());
        }
    }

    private static IServiceCollection CreateEmptyServiceCollection()
    {
        return new ServiceCollection()
            .AddLogging(builder => builder.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(new MongoClient());
    }
}