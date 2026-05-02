using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Bson;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
namespace Mongo.Migration.Startup;

public static class MongoMigrationExtensions
{
    public static IServiceCollection AddMigration(
        this IServiceCollection services,
        Action<MongoMigrationConfigurator>? configure = null)
    {
        MongoMigrationConfigurator configurator = new();

        configure?.Invoke(configurator);

        configurator.AddAllMigrationsIfNothingWasAdded();

        BuildDocumentMigration(services, configurator);

        BuildDatabaseMigration(services, configurator);

        services
            .AddSingleton(configurator.MongoMigrationSettings)
            .AddSingleton(configurator.MongoMigrationStartupSettings)
            .AddTransient<IMigrationService, MigrationService>();

        return services;
    }

    private static void BuildDocumentMigration(IServiceCollection services, MongoMigrationConfigurator migrationConfigurator)
    {
        if (migrationConfigurator.MongoMigrationStartupSettings.RuntimeMigrationEnabled ||
            migrationConfigurator.MongoMigrationStartupSettings.StartupDocumentMigrationEnabled)
        {
            services
                .AddSingleton<IMigrationLocator<IDocumentMigration>, TypeMigrationLocator>()
                .AddSingleton<IRuntimeVersionLocator>(new RuntimeVersionLocator(migrationConfigurator.RuntimeMigrationDictionary))
                .AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>()
                .AddSingleton<IDocumentVersionService, DocumentVersionService>()
                .AddSingleton<IDocumentMigrationRunner, DocumentMigrationRunner>();

            if (migrationConfigurator.MongoMigrationStartupSettings.RuntimeMigrationEnabled)
            {
                services
                    .AddSingleton<MigrationBsonSerializerProvider>();
            }

            if (migrationConfigurator.MongoMigrationStartupSettings.StartupDocumentMigrationEnabled)
            {
                services
                    .AddTransient<ICollectionLocator, CollectionLocator>()
                    .AddTransient<IStartUpDocumentMigrationRunner, StartUpDocumentMigrationRunner>();
            }
        }
    }

    private static void BuildDatabaseMigration(IServiceCollection services, MongoMigrationConfigurator migrationConfigurator)
    {
        if (migrationConfigurator.MongoMigrationStartupSettings.DatabaseMigrationEnabled)
        {
            services
                .AddTransient<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>()
                .AddTransient<IDatabaseVersionService, DatabaseVersionService>()
                .AddTransient<IDatabaseMigrationRunner, DatabaseMigrationRunner>();
        }
    }
}