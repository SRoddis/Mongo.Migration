using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Migration;
using Mongo.Migration.Services.Migration.OnDeserialization;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using Mongo.Migration.Services.Migration.OnStartup;

namespace Mongo.Migration.Services.Startup.DotNetCore
{
    public static class MongoMigrationExtensions
    {
        public static void AddMigrationOnDeserialization(
            this IServiceCollection services)
        {
            services.AddScoped<DocumentVersionSerializer, DocumentVersionSerializer>();
            services.AddScoped<MigrationInterceptorProvider, MigrationInterceptorProvider>();
            
            services.AddScoped<IMigrationStrategy, MigrationOnDeserialization>();
            
            RegisterDefaults(services);
        }
        
        public static void AddMigrationOnStartup(
            this IServiceCollection services)
        {
            services.AddScoped<IMigrationStrategy, MigrationOnStartup>();
            
            RegisterDefaults(services);
        }
        
        private static void RegisterDefaults(IServiceCollection services)
        {
            services.AddSingleton<IMigrationLocator, TypeMigrationLocator>();
            services.AddSingleton<IVersionLocator, VersionLocator>();

            services.AddScoped<IMigrationRunner, MigrationRunner>();
            services.AddScoped<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            
            services.AddScoped<IMongoMigration, MongoMigration>();
        }
    }
}
