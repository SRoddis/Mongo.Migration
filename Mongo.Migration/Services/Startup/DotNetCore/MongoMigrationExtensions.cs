using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Migration;
using Mongo.Migration.Services.Migration.OnDeserialization;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;

namespace Mongo.Migration.Services.Startup.DotNetCore
{
    public static class MongoMigrationExtensions
    {
        public static void AddMongoMigration(
            this IServiceCollection services)
        {
            services.AddScoped<DocumentVersionSerializer, DocumentVersionSerializer>();
            services.AddScoped<MigrationInterceptorProvider, MigrationInterceptorProvider>();
            services.AddSingleton<IMigrationLocator, TypeMigrationLocator>();
            services.AddSingleton<IVersionLocator, VersionLocator>();

            services.AddScoped<IMigrationRunner, MigrationRunner>();
            services.AddScoped<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            services.AddScoped<IMigrationStrategy, MigrationOnDeserialization>();
            services.AddScoped<IMongoMigration, MongoMigration>();
        }
    }
}
