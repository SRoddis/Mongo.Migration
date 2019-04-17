using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Migration;
using Mongo.Migration.Services.Migration.OnDeserialization;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using Mongo.Migration.Services.Migration.OnStartup;

namespace Mongo.Migration.Startup.DotNetCore
{
    public static class MongoMigrationExtensions
    {
        public static void AddMigrationOnStartup(
            this IServiceCollection services)
        {
            RegisterDefaults(services);

            services.AddScoped<ICollectionMigrationRunner, CollectionMigrationRunner>();
            services.AddScoped<IMigrationStrategy, MigrationOnStartup>();
        }
        
        public static void AddMigrationOnDeserialization(
            this IServiceCollection services)
        {
            RegisterDefaults(services);
            
            services.AddScoped<IMigrationRunner, MigrationRunner>();
            services.AddScoped<IMigrationStrategy, MigrationOnDeserialization>();            
            services.AddScoped<MigrationInterceptorProvider, MigrationInterceptorProvider>();
        }
        
        private static void RegisterDefaults(IServiceCollection services)
        {
            services.AddSingleton<IMigrationLocator, TypeMigrationLocator>();
            services.AddSingleton<ICollectionLocator, CollectionLocator>();
            services.AddSingleton<IVersionLocator, VersionLocator>();

            services.AddScoped<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            services.AddScoped<DocumentVersionSerializer, DocumentVersionSerializer>();
            
            services.AddScoped<IMongoMigration, MongoMigration>();
        }
    }
}
