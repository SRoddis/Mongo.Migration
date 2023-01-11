using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;

namespace Mongo.Migration.Startup.DotNetCore
{
    public static class MongoMigrationExtensions
    {
        public static void AddMigration(this IServiceCollection services, IMongoMigrationSettings settings = null)
        {
            RegisterDefaults(services, settings ?? new MongoMigrationSettings());

            services.AddScoped<IMigrationService, MigrationService>();
        }

        public static void AddMigrationStartupFilter(this IServiceCollection services) => services.AddTransient<IStartupFilter, MongoMigrationStartupFilter>();

        private static void RegisterDefaults(IServiceCollection services, IMongoMigrationSettings settings)
        {
            services.AddSingleton(settings);

            services.AddSingleton<IContainerProvider, Migrations.Adapters.ServiceProvider>();
            services.AddSingleton(typeof(IMigrationLocator<>), typeof(TypeMigrationDependencyLocator<>));
            services.AddSingleton<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>();
            services.AddSingleton<ICollectionLocator, CollectionLocator>();
            services.AddSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>();
            services.AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>();

            services.AddTransient<IDatabaseVersionService, DatabaseVersionService>();
            services.AddTransient<IDocumentVersionService, DocumentVersionService>();
            services.AddTransient<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            services.AddTransient<DocumentVersionSerializer, DocumentVersionSerializer>();

            services.AddTransient<IStartUpDocumentMigrationRunner, StartUpDocumentMigrationRunner>();
            services.AddTransient<IDocumentMigrationRunner, DocumentMigrationRunner>();

            services.AddTransient<IStartUpDatabaseMigrationRunner, StartUpDatabaseMigrationRunner>();
            services.AddTransient<IDatabaseMigrationRunner, DatabaseMigrationRunner>();

            services.AddTransient<IMigrationInterceptorProvider, MigrationInterceptorProvider>();

            services.AddTransient<IMongoMigration, MongoMigration>();
        }
    }
}