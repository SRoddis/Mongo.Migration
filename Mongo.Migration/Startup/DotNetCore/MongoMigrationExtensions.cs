using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;

namespace Mongo.Migration.Startup.DotNetCore
{
    public static class MongoMigrationExtensions
    {
        public static void AddMigration(
            this IServiceCollection services)
        {
            RegisterDefaults(services);

            services.AddScoped<IMigrationService, MigrationService>();
        }

        private static void RegisterDefaults(IServiceCollection services)
        {
            services.AddSingleton<IMigrationLocator, TypeMigrationLocator>();
            services.AddSingleton<ICollectionLocator, CollectionLocator>();
            services.AddSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>();
            services.AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>();

            services.AddTransient<IVersionService, VersionService>();
            services.AddTransient<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            services.AddTransient<DocumentVersionSerializer, DocumentVersionSerializer>();

            services.AddTransient<ICollectionMigrationRunner, CollectionMigrationRunner>();
            services.AddTransient<IMigrationRunner, MigrationRunner>();
            services.AddTransient<MigrationInterceptorProvider, MigrationInterceptorProvider>();

            services.AddTransient<IMongoMigration, MongoMigration>();
            services.AddTransient<IStartupFilter, MongoMigrationStartupFilter>();
        }
    }
}