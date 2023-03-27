using System;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;

namespace Mongo.Migration.Startup
{
    public static class MongoMigrationExtensions
    {
        public static IServiceCollection AddMigration(this IServiceCollection services, Action<MongoMigrationSettings> settings = null)
        {
            var migrationSettings = new MongoMigrationSettings();
            settings?.Invoke(migrationSettings);
            services.RegisterDefaults(migrationSettings);
            services.AddSingleton<IMigrationService, MigrationService>();
            services.AddHostedService<MongoMigrationHostedService>();
            
            return services;
        }

        private static void RegisterDefaults(this IServiceCollection services, IMongoMigrationSettings settings)
        {
            services.AddSingleton(settings);
            services.AddSingleton<IMongoMigrationAssemblyService, MongoMigrationAssemblyService>();
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