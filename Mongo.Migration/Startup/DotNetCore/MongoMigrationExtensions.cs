using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;
using ServiceProvider = Mongo.Migration.Migrations.Adapters.ServiceProvider;

namespace Mongo.Migration.Startup.DotNetCore
{
    public static class MongoMigrationExtensions
    {
        public static void AddMigration(
            this IServiceCollection services, IMongoMigrationSettings settings = null)
        {
            RegisterDefaults(services, settings ?? new MongoMigrationSettings());

            services.AddScoped<IMigrationService, MigrationService>();
        }

        private static void RegisterDefaults(IServiceCollection services, IMongoMigrationSettings settings)
        {
            services.AddSingleton(settings);
            
            services.AddSingleton<IContainerProvider, ServiceProvider>();
            services.AddSingleton<IMigrationLocator, TypeMigrationDependencyLocator>();
            services.AddSingleton<ICollectionLocator, CollectionLocator>();
            services.AddSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>();
            services.AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>();

            services.AddTransient<IVersionService, VersionService>();
            services.AddTransient<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            services.AddTransient<DocumentVersionSerializer, DocumentVersionSerializer>();

            services.AddTransient<ICollectionMigrationRunner, CollectionMigrationRunner>();
            services.AddTransient<IMigrationRunner, MigrationRunner>();
            
            services.AddTransient<IMigrationInterceptorProvider, MigrationInterceptorProvider>();

            services.AddTransient<IMongoMigration, MongoMigration>();
            services.AddTransient<IStartupFilter, MongoMigrationStartupFilter>();
        }
    }
}